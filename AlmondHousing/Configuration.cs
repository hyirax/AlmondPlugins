using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Configuration;

namespace AlmondHousing
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        // 馃殌 銆愬繀椤绘柊澧炵殑杩欎竴琛屻€戯細鐢ㄤ簬姘镐箙淇濆瓨闃插€掑崠绯荤粺鐨勮В閿佺姸鎬?
        public bool IsActivated { get; set; } = false;

        // 鐢ㄤ簬淇濆瓨鐜╁閫夋嫨鐨勮瑷€锛岄粯璁よ涓?"zh" (涓枃)
        public string UILanguage { get; set; } = "zh"; // 榛樿浣跨敤绠€浣撲腑鏂?
        
        public bool ShowTooltips = true;
        public bool DrawScreen = false;
        public float DrawDistance = 0;
        public List<int> HiddenScreenItemHistory = new List<int>();
        public List<int> GroupingList = new List<int>();
        
        public bool Basement = true;
        public bool GroundFloor = true;
        public bool UpperFloor = true;

        public int LoadInterval = 400;
        public bool ApplyLayout = true;
        public bool EnableQuantumPlace = false;
        public bool AbsoluteSnap = false;
        public bool UseGizmo = false;    // 鏄惁鍦ㄥ睆骞曚笂鏄剧ず 3D 鎷栨嫿杞?
        public bool DoSnap = false;      // 鏄惁寮€鍚嫋鎷藉惛闄?
        public float Drag = 0.05f;       // 榧犳爣婊氳疆/鎷栨嫿鐨勫井璋冪簿搴?

        public string SaveLocation = null;

        [NonSerialized]
        private CancellationTokenSource _saveCts;
        private static readonly TimeSpan SaveDebounceInterval = TimeSpan.FromMilliseconds(500);

        public void Save()
        {
            _saveCts?.Cancel();
            _saveCts = new CancellationTokenSource();
            var cts = _saveCts;
            Task.Delay(SaveDebounceInterval, cts.Token).ContinueWith(t =>
            {
                if (!cts.IsCancellationRequested && !t.IsFaulted)
                    DalamudApi.PluginInterface.SavePluginConfig(this);
            }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);
        }

        public void ResetRecord()
        {
            HiddenScreenItemHistory.Clear();
            GroupingList.Clear();
            Save();
        }
    }
}


