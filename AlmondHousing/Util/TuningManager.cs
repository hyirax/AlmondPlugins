using System;
using System.Collections.Generic;
using System.Numerics;

namespace AlmondHousing
{
    public class TransformChange 
    {
        public nint ItemPtr;
        public Vector3 OldPos;
        public Vector3 NewPos;
    }

    public class ActionSnapshot 
    {
        public List<TransformChange> Changes = new();
    }

    public static class TuningManager
    {
        private static List<ActionSnapshot> UndoStack = new();
        private static int UndoIndex = -1;

        public static HashSet<nint> GroupedItems = new();

        public static unsafe void PushUndo(ActionSnapshot action) 
        {
            if (UndoIndex < UndoStack.Count - 1) 
            {
                UndoStack.RemoveRange(UndoIndex + 1, UndoStack.Count - (UndoIndex + 1));
            }
            UndoStack.Add(action);
            
            if (UndoStack.Count > Constants.Housing.MaxHistorySize) { UndoStack.RemoveAt(0); } 
            else { UndoIndex++; }
        }

        public static unsafe void Undo() 
        {
            if (UndoIndex >= 0) 
            {
                var action = UndoStack[UndoIndex];
                foreach(var c in action.Changes) 
                {
                    if (c.ItemPtr != IntPtr.Zero) 
                    {
                        *(Vector3*)(c.ItemPtr + Constants.Offsets.ItemActivePosition) = c.OldPos; 
                        Memory.Instance.HousingLayoutModelUpdate?.Invoke(c.ItemPtr + Constants.Offsets.ItemModelRefresh);
                    }
                }
                UndoIndex--;
            }
        }

        public static unsafe void Redo() 
        {
            if (UndoIndex < UndoStack.Count - 1) {
                UndoIndex++;
                var action = UndoStack[UndoIndex];
                foreach(var c in action.Changes) 
                {
                    if (c.ItemPtr != IntPtr.Zero) {
                        *(Vector3*)(c.ItemPtr + Constants.Offsets.ItemActivePosition) = c.NewPos;
                        Memory.Instance.HousingLayoutModelUpdate?.Invoke(c.ItemPtr + Constants.Offsets.ItemModelRefresh);
                    }
                }
            }
        }

        public static void ClearGroup() => GroupedItems.Clear();
    }
}