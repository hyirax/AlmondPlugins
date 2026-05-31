using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using Lumina.Excel.Sheets;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace AlmondHousing
{
    public unsafe class Memory
    {
        public static GetInventoryContainerDelegate GetInventoryContainer;
        public delegate InventoryContainer* GetInventoryContainerDelegate(IntPtr inventoryManager, InventoryType inventoryType);

        public IntPtr placeAnywhere;
        public IntPtr wallAnywhere;
        public IntPtr wallmountAnywhere;
        public IntPtr showcasePlaceAddress;
        public IntPtr showcaseRotateAddress;

        // 🚀【第四阶段：安全气囊 1】绝对路径强锁定，彻底消灭 CS0104 冲突与 CS8500 指针警告
#pragma warning disable CS8500
        public static unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Camera* Camera
        {
            get
            {
                try
                {
                    var manager = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager.Instance();
                    if (manager == null) return null;
                    var activeCam = manager->GetActiveCamera();
                    if (activeCam == null) return null;
                    return &activeCam->CameraBase.SceneCamera;
                }
                catch { return null; }
            }
        }
#pragma warning restore CS8500

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void HousingLayoutModelUpdateDelegate(nint item);
        public HousingLayoutModelUpdateDelegate HousingLayoutModelUpdate = null!;

        private byte[] showcasePlaceOriginal;
        private byte[] showcaseRotateOriginal;
        private byte[] showcasePlaceNoop;
        private byte[] showcaseRotateNoop;

        private Memory()
        {
            try
            {
                placeAnywhere = DalamudApi.SigScanner.ScanText(Constants.Signatures.PlaceAnywhere) + Constants.Offsets.PlaceAnywhereAdjustment;
                wallAnywhere = DalamudApi.SigScanner.ScanText(Constants.Signatures.WallAnywhere) + Constants.Offsets.WallAnywhereAdjustment;
                wallmountAnywhere = DalamudApi.SigScanner.ScanText(Constants.Signatures.WallmountAnywhere) + Constants.Offsets.WallmountAnywhereAdjustment;

                showcasePlaceAddress = DalamudApi.SigScanner.ScanText(Constants.Signatures.ShowcasePlace);
                showcaseRotateAddress = DalamudApi.SigScanner.ScanText(Constants.Signatures.ShowcaseRotate);

                if (showcasePlaceAddress != IntPtr.Zero && showcaseRotateAddress != IntPtr.Zero)
                {
                    showcasePlaceOriginal = ReadBytes(showcasePlaceAddress, 7);
                    showcaseRotateOriginal = ReadBytes(showcaseRotateAddress, 6);
                    
                    showcasePlaceNoop = Enumerable.Repeat((byte)0x90, showcasePlaceOriginal.Length).ToArray();
                    showcaseRotateNoop = Enumerable.Repeat((byte)0x90, showcaseRotateOriginal.Length).ToArray();
                }

                housingModulePtr = DalamudApi.SigScanner.GetStaticAddressFromSig(Constants.Signatures.HousingModule);
                LayoutWorldPtr = DalamudApi.SigScanner.GetStaticAddressFromSig(Constants.Signatures.LayoutWorld, Constants.Offsets.LayoutWorldInstructionLength);

                var getInventoryContainerPtr = DalamudApi.SigScanner.ScanText(Constants.Signatures.GetInventoryContainer);
                GetInventoryContainer = Marshal.GetDelegateForFunctionPointer<GetInventoryContainerDelegate>(getInventoryContainerPtr);

                var housingLayoutModelUpdateAddress = DalamudApi.SigScanner.ScanText(Constants.Signatures.HousingLayoutModelUpdate);
                HousingLayoutModelUpdate = Marshal.GetDelegateForFunctionPointer<HousingLayoutModelUpdateDelegate>(housingLayoutModelUpdateAddress);
            }
            catch (Exception e)
            {
                DalamudApi.PluginLog.Error(e, "Could not load housing memory!!");
            }
        }

        public static Memory Instance { get; private set; }
        public IntPtr housingModulePtr { get; }
        public IntPtr LayoutWorldPtr { get; }

        // 🚀【第四阶段：安全气囊 2】给高危的原生托管指针全部加装空检验防护锁，彻底杜绝游戏切地图或断开联结时直接闪退（CTD）
        public unsafe HousingModule* HousingModule => housingModulePtr != IntPtr.Zero ? (HousingModule*)Marshal.ReadIntPtr(housingModulePtr) : null;
        public unsafe LayoutWorld* LayoutWorld => LayoutWorldPtr != IntPtr.Zero ? (LayoutWorld*)Marshal.ReadIntPtr(LayoutWorldPtr) : null;
        public unsafe HousingObjectManager* CurrentManager => (HousingModule != null && HousingModule->currentTerritory != null) ? HousingModule->currentTerritory : null;
        public unsafe HousingStructure* HousingStructure => (LayoutWorld != null && LayoutWorld->HousingStruct != null) ? LayoutWorld->HousingStruct : null;

        public static void Init() => Instance = new Memory();
        public static InventoryContainer* GetContainer(InventoryType inventoryType) => InventoryManager.Instance()->GetInventoryContainer(inventoryType);
        public uint GetTerritoryTypeId() => GetActiveLayout(out var manager) ? manager.TerritoryTypeId : 0;
        public bool HasUpperFloor() => GetIndoorHouseSize() == "Medium" || GetIndoorHouseSize() == "Large";

        public string GetIndoorHouseSize()
        {
            var territoryId = GetTerritoryTypeId();
            var row = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(territoryId);
            if (row.RowId == 0) return null;
            var placeName = row.Name.ToString();
            if (string.IsNullOrEmpty(placeName) || placeName.Length < 4) return null;
            var sizeName = placeName.Substring(2, 2);
            return sizeName switch { "i1" => "Small", "i2" => "Medium", "i3" => "Large", "i4" => "Apartment", _ => null };
        }

        public float GetInteriorLightLevel() => (GetCurrentTerritory() == HousingArea.Indoors && GetActiveLayout(out var m) && m.IndoorAreaData.HasValue) ? m.IndoorAreaData.Value.LightLevel : 0f;

        public CommonFixture[] GetInteriorCommonFixtures(int floorId)
        {
            if (GetCurrentTerritory() != HousingArea.Indoors || !GetActiveLayout(out var m) || !m.IndoorAreaData.HasValue) return new CommonFixture[0];
            var floor = m.IndoorAreaData.Value.GetFloor(floorId);
            var ret = new CommonFixture[IndoorFloorData.PartsMax];
            for (var i = 0; i < IndoorFloorData.PartsMax; i++)
            {
                var key = floor.GetPart(i);
                if (!HousingData.Instance.TryGetItem(unchecked((uint)key), out var item)) HousingData.Instance.IsUnitedExteriorPart(unchecked((uint)key), out item);
                ret[i] = new CommonFixture(false, i, key, null, item);
            }
            return ret;
        }

        public CommonFixture[] GetExteriorCommonFixtures(int plotId)
        {
            if (GetCurrentTerritory() != HousingArea.Outdoors || !GetHousingController(out var c)) return new CommonFixture[0];
            var home = c.Houses(plotId);
            if (home.Size == -1 || home.GetPart(0).Category == -1) return new CommonFixture[0];
            var ret = new CommonFixture[HouseCustomize.PartsMax];
            for (var i = 0; i < HouseCustomize.PartsMax; i++)
            {
                var colorId = home.GetPart(i).Color;
                HousingData.Instance.TryGetStain(colorId, out var stain);
                HousingData.Instance.TryGetItem(home.GetPart(i).FixtureKey, out var item);
                ret[i] = new CommonFixture(true, home.GetPart(i).Category, home.GetPart(i).FixtureKey, stain, item);
            }
            return ret;
        }
        
        public unsafe bool GetExteriorPlacedObjects(out List<HousingGameObject> objects, Vector3 playerPos) 
        {
            objects = null;
            if (HousingModule == null || HousingModule->GetCurrentManager() == null || HousingModule->GetCurrentManager()->Objects == null) return false;
            var tmpObjects = new List<(HousingGameObject gObject, float distance)>(400);
            objects = new List<HousingGameObject>(400);
            var curMgr = HousingModule->outdoorTerritory;
            nint* objectsArrayPtr = (nint*)curMgr->Objects;
            ReadOnlySpan<nint> pointerSpan = new ReadOnlySpan<nint>(objectsArrayPtr, 400);
            foreach (ref readonly nint oPtr in pointerSpan)
            {
                if (oPtr == 0) continue;
                var o = *(HousingGameObject*)oPtr;
                tmpObjects.Add((o, Utils.DistanceFromPlayer(o, playerPos)));
            }
            tmpObjects.Sort((obj1, obj2) => obj1.distance.CompareTo(obj2.distance));
            foreach (var item in tmpObjects) objects.Add(item.gObject);
            return true;
        }

        public unsafe bool TryGetIslandGameObjectList(out List<HousingGameObject> objects)
        {
            objects = new List<HousingGameObject>(200);
            var mjiManager = MJIManager.Instance();
            if (mjiManager == null) return false;
            var manager = (MjiManagerExtended*)mjiManager;
            var objectManager = manager->ObjectManager;
            if (objectManager == null) return false;
            var furnManager = objectManager->FurnitureManager;
            if (furnManager == null) return false;
            nint* objectsArrayPtr = (nint*)furnManager->Objects;
            ReadOnlySpan<nint> pointerSpan = new ReadOnlySpan<nint>(objectsArrayPtr, 200);
            foreach (ref readonly nint objPtr in pointerSpan)
            {
                if (objPtr == 0) continue;
                objects.Add(*(HousingGameObject*)objPtr);
            }
            return true;
        }

        public unsafe bool TryGetNameSortedHousingGameObjectList(out List<HousingGameObject> objects)
        {
            objects = new List<HousingGameObject>();
            if (HousingModule == null || HousingModule->GetCurrentManager() == null || HousingModule->GetCurrentManager()->Objects == null) return false;
            var manager = HousingModule->GetCurrentManager();
            nint* objectsArrayPtr = (nint*)manager->Objects; 
            ReadOnlySpan<nint> pointerSpan = new ReadOnlySpan<nint>(objectsArrayPtr, 600);
            var tempList = new List<(HousingGameObject Obj, string Name)>(600);
            foreach (ref readonly nint oPtr in pointerSpan)
            {
                if (oPtr == 0) continue;
                var o = *(HousingGameObject*)oPtr;
                string itemName = "";
                if (HousingData.Instance.TryGetFurniture(o.housingRowId, out var furniture)) itemName = furniture.Item.Value.Name.ToString(); 
                else if (HousingData.Instance.TryGetYardObject(o.housingRowId, out var yardObject)) itemName = yardObject.Item.Value.Name.ToString();
                tempList.Add((o, itemName));
            }
            tempList.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            objects.Capacity = tempList.Count;
            foreach (var item in tempList) objects.Add(item.Obj);
            return true;
        }

        public unsafe bool GetActiveLayout(out LayoutManager manager)
        {
            manager = new LayoutManager();
            if (LayoutWorld == null || LayoutWorld->ActiveLayout == null) return false;
            manager = *LayoutWorld->ActiveLayout;
            return true;
        }

        public bool GetHousingController(out HousingController controller)
        {
            controller = new HousingController();
            if (!GetActiveLayout(out var manager) || !manager.HousingController.HasValue) return false;
            controller = manager.HousingController.Value;
            return true;
        }

        public enum HousingArea { Indoors, Outdoors, Island, None }

        public unsafe HousingArea GetCurrentTerritory()
        {
            var territoryRow = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(GetTerritoryTypeId());
            if (territoryRow.RowId == 0 || territoryRow.Name.ToString().Equals("r1i5")) return HousingArea.None;
            if (territoryRow.Name.ToString().Equals("h1m2")) return HousingArea.Island;
            if (HousingModule == null) return HousingArea.None;
            return HousingModule->IsOutdoors() ? HousingArea.Outdoors : HousingArea.Indoors;
        }

        public unsafe bool IsHousingMode() => HousingStructure != null && HousingStructure->Mode != HousingLayoutMode.None;
        public unsafe bool CanEditItem() => HousingStructure != null && HousingStructure->Mode == HousingLayoutMode.Rotate;

        public unsafe void WritePosition(Vector3 newPosition)
        {
            if (!CanEditItem()) return;
            try { var item = HousingStructure->ActiveItem; if (item != null) item->Position = newPosition; }
            catch (Exception ex) { DalamudApi.PluginLog.Error(ex, "Error occured while writing position!"); }
        }

        public unsafe void WriteRotation(Vector3 newRotation)
        {
            if (!CanEditItem()) return;
            try { var item = HousingStructure->ActiveItem; if (item != null) item->Rotation = MoveUtil.ToQ(newRotation); }
            catch (Exception ex) { DalamudApi.PluginLog.Error(ex, "Error occured while writing rotation!"); }
        }

        public void SetPlaceAnywhere(bool state)
        {
            if (placeAnywhere == IntPtr.Zero || wallAnywhere == IntPtr.Zero || wallmountAnywhere == IntPtr.Zero) return;

            var bstate = (byte)(state ? 1 : 0);
            WriteProtectedBytes(placeAnywhere, bstate);
            WriteProtectedBytes(wallAnywhere, bstate);
            WriteProtectedBytes(wallmountAnywhere, bstate);

            if (showcasePlaceAddress != IntPtr.Zero && showcaseRotateAddress != IntPtr.Zero && showcasePlaceOriginal != null)
            {
                WriteProtectedBytes(showcasePlaceAddress, state ? showcasePlaceNoop : showcasePlaceOriginal);
                WriteProtectedBytes(showcaseRotateAddress, state ? showcaseRotateNoop : showcaseRotateOriginal);
            }
        }

        
        private static void WriteProtectedBytesBatched(List<(IntPtr addr, byte[] bytes)> writes)
        {
            var oldProtections = new Dictionary<IntPtr, Protection>();
            foreach (var (addr, bytes) in writes)
            {
                if (addr == IntPtr.Zero || bytes == null) continue;
                VirtualProtect(addr, (uint)bytes.Length, Protection.PAGE_EXECUTE_READWRITE, out var oldProt);
                oldProtections[addr] = oldProt;
            }
            foreach (var (addr, bytes) in writes)
            {
                if (addr == IntPtr.Zero || bytes == null) continue;
                Marshal.Copy(bytes, 0, addr, bytes.Length);
            }
            foreach (var (addr, bytes) in writes)
            {
                if (addr == IntPtr.Zero || bytes == null || !oldProtections.TryGetValue(addr, out var oldProt)) continue;
                VirtualProtect(addr, (uint)bytes.Length, oldProt, out _);
            }
        }

        private static byte[] ReadBytes(IntPtr addr, int length)
        {
            var bytes = new byte[length];
            Marshal.Copy(addr, bytes, 0, length);
            return bytes;
        }

        private static void WriteProtectedBytes(IntPtr addr, byte[] b)
        {
            if (addr == IntPtr.Zero) return;
            VirtualProtect(addr, 1, Protection.PAGE_EXECUTE_READWRITE, out var oldProtection);
            Marshal.Copy(b, 0, addr, b.Length);
            VirtualProtect(addr, 1, oldProtection, out _);
        }

        private static void WriteProtectedBytes(IntPtr addr, byte b)
        {
            if (addr == IntPtr.Zero) return;
            WriteProtectedBytes(addr, [b]);
        }

        #region Kernel32
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, Protection flNewProtect, out Protection lpflOldProtect);

        public enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }
        #endregion
    }
}
