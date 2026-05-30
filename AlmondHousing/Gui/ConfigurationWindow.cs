using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Utility;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using AlmondHousing.Objects;
using AlmondHousing.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using static AlmondHousing.AlmondHousing;
using Dalamud.Interface.Textures;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;

namespace AlmondHousing.Gui
{
    public partial class ConfigurationWindow : Window
    {
        private AlmondHousing Plugin;
        public Configuration Config => Plugin.Config;

        public bool CanUpload { get; set; }
        public bool CanImport { get; set; }

        private bool showOnlyMisplaced = false;
        private bool showOnlyMissing = false;

        private string CustomTag = string.Empty;
        private readonly Dictionary<uint, uint> iconToFurniture = new() { };

        private readonly Vector4 THEME_BASE = new(0.20f, 0.20f, 0.20f, 1f);     
        private readonly Vector4 THEME_HOVER = new(0.35f, 0.35f, 0.35f, 1f);    
        private readonly Vector4 THEME_ACTIVE = new(0.45f, 0.45f, 0.45f, 1f);   
        private readonly Vector4 THEME_HEADER = new(0.15f, 0.15f, 0.15f, 0.8f);
        private readonly Vector4 ACCENT_COLOR = new(0.9f, 0.7f, 0.3f, 1.0f); 

        private FileDialogManager FileDialogManager { get; }
        private Dalamud.Game.ClientLanguage currentExportLang = Dalamud.Game.ClientLanguage.English;
        private string searchQuery = string.Empty;

        private int selectedTab = 0;
        // Animated collapsing header states
        private bool _interiorFurnitureOpen = true;
        private bool _exteriorFurnitureOpen = true;
        private bool _unusedFurnitureOpen = true;
        private bool _interiorFixturesOpen = true;
        private bool _exteriorFixturesOpen = true;

        
        private Dictionary<int, float> _sidebarAnimStates = new Dictionary<int, float>();
        private string _antiResellInput = string.Empty;
        
        private Dictionary<int, Vector2> _stableScreenCoords = new Dictionary<int, Vector2>();

        public ConfigurationWindow(AlmondHousing plugin) : base("AlmondHousing", ImGuiWindowFlags.NoScrollWithMouse)
        {
            Plugin = plugin;
            this.SizeCondition = ImGuiCond.FirstUseEver;
            this.Size = new Vector2(800, 550);
            this.FileDialogManager = new FileDialogManager
            {
                AddedWindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking,
            };
        }

        private string GetAntiResellPhrase()
        {
            return Config.UILanguage switch
            {
                "zh" => "插件免费倒卖狗死个妈买的和卖的全是傻逼",
                "zh_TW" => "外掛免費倒賣狗死全家買的和賣的全是傻逼",
                "ja" => "このプラグインは無料です、転売屋は詐欺です",
                "ko" => "이 플러그인은 무료입니다, 되팔이는 사기꾼입니다",
                "de" => "Dieses Plugin ist kostenlos, Wiederverkäufer sind Betrüger",
                "fr" => "Ce plugin est gratuit, les revendeurs sont des escrocs",
                _ => "This plugin is free, resellers are scammers"
            };
        }

        private string GetAntiResellInstruction()
        {
            return Config.UILanguage switch
            {
                "zh" => "严禁倒卖！为确认您未被倒卖狗欺骗，请【手动完整输入】以下红色口令以解锁插件：",
                "zh_TW" => "嚴禁倒賣！為確認您未被倒賣狗欺骗，請【手動完整輸入】以下紅色口令以解鎖外掛：",
                "ja" => "転売禁止！詐欺被害を防ぐため、以下の赤い宣言文を【手動で正確に入力】してプラグインのロックを解除してください：",
                "ko" => "되팔이 금지! 사기 피해를 방지하려면 아래의 빨간색 문구를 【정확히 직접 입력】하여 플러그인을 잠금 해제하세요:",
                "de" => "Verkauf verboten! Um zu bestätigen, dass Sie nicht betrogen wurden, tippen Sie den folgenden roten Satz manuell ein:",
                "fr" => "Revente interdite ! Pour confirmer que vous n'avez pas été arnaqué, veuillez taper manuellement la phrase rouge ci-dessous :",
                _ => "Anti-Resell Check! To confirm you weren't scammed, please type the red phrase below exactly as shown to unlock the plugin:"
            };
        }

        public override void PreDraw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f); 
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0f); 
            ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, 0f); 
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 12.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 6.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 8.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 8.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10)); 
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(8, 6));    

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.05f, 0.05f, 0.05f, 0.95f)); 
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));      
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new Vector4(0.08f, 0.08f, 0.08f, 0.98f));  
            ImGui.PushStyleColor(ImGuiCol.TitleBg, new Vector4(0.02f, 0.02f, 0.02f, 0.98f));  
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.08f, 0.08f, 0.08f, 0.98f)); 
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));       
            
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.15f, 0.15f, 0.15f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.22f, 0.22f, 0.22f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0.28f, 0.28f, 0.28f, 1.0f));
            
            ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.18f, 0.18f, 0.18f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.25f, 0.25f, 0.25f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0.32f, 0.32f, 0.32f, 1.0f));

            ImGui.PushStyleColor(ImGuiCol.TableHeaderBg, new Vector4(0.14f, 0.14f, 0.15f, 0.9f));     
            ImGui.PushStyleColor(ImGuiCol.TableBorderStrong, new Vector4(0.2f, 0.2f, 0.22f, 0.4f));    
            ImGui.PushStyleColor(ImGuiCol.TableBorderLight, new Vector4(0.15f, 0.15f, 0.16f, 0.3f));   

            ImGui.PushStyleColor(ImGuiCol.Button, THEME_BASE);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, THEME_HOVER);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, THEME_ACTIVE);
            
            ImGui.PushStyleColor(ImGuiCol.ScrollbarBg, new Vector4(0.02f, 0.02f, 0.02f, 0.5f));
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab, THEME_HOVER);
            ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabHovered, ACCENT_COLOR);

            ImGui.PushStyleColor(ImGuiCol.CheckMark, ACCENT_COLOR); 

            ImGui.PushStyleColor(ImGuiCol.ResizeGrip, THEME_HOVER);
            ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered, ACCENT_COLOR);
            ImGui.PushStyleColor(ImGuiCol.ResizeGripActive, ACCENT_COLOR);
        }

        public override void PostDraw()
        {
            ImGui.PopStyleVar(9); 
            ImGui.PopStyleColor(25); 
        }

        public override void Draw()
        {
            ListenForCheatCode();

            if (!Config.IsActivated)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.12f, 0.05f, 0.05f, 1.0f)); 
                ImGui.BeginChild("AntiResellLock", new Vector2(0, 0), true);
                
                ImGui.SetCursorPosY(20);
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - 90);
                
                string[] supportedLangNames = { "简体中文", "繁體中文", "English", "日本語", "한국어", "Deutsch", "Français" };
                string[] supportedLangs = { "zh", "zh_TW", "en", "ja", "ko", "de", "fr" };
                int currentLangIndex = Math.Max(0, Array.IndexOf(supportedLangs, Config.UILanguage));

                ImGui.SetNextItemWidth(180); 
                if (ImGui.BeginCombo("##AntiResellLang", supportedLangNames[currentLangIndex]))
                {
                    for (int i = 0; i < supportedLangs.Length; i++)
                    {
                        DrawInlineIconColored(FontAwesomeIcon.Globe, ACCENT_COLOR); ImGui.SameLine();
                        if (ImGui.Selectable(supportedLangNames[i], currentLangIndex == i))
                        {
                            Config.UILanguage = supportedLangs[i];
                            Config.Save();
                            Lang.SetLanguage(Config.UILanguage);
                        }
                    }
                    ImGui.EndCombo();
                }

                ImGui.SetCursorPosY(ImGui.GetWindowHeight() / 2 - 120);
                
                float pulseAlpha = (float)(Math.Sin(ImGui.GetTime() * 4.0) + 1.0) / 2.0f;
                Vector4 warningColor = new Vector4(1.0f, 0.3f, 0.3f, 0.6f + (pulseAlpha * 0.4f));
                
                string titleText = Lang.GetText("AntiResellTitle");
                float titleWidth = ImGui.CalcTextSize(titleText).X + 25; 
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - titleWidth / 2);
                DrawInlineIconColored(FontAwesomeIcon.ShieldAlt, warningColor);
                ImGui.SameLine();
                ImGui.TextColored(warningColor, titleText);
                
                ImGui.Dummy(new Vector2(0, 15));
                
                string instruction = GetAntiResellInstruction();
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - ImGui.CalcTextSize(instruction).X / 2);
                ImGui.Text(instruction);
                
                ImGui.Dummy(new Vector2(0, 10));

                string expectedPhrase = GetAntiResellPhrase();
                
                ImGui.PushFont(UiBuilder.DefaultFont); 
                string displayPhrase = $"【 {expectedPhrase} 】";
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - ImGui.CalcTextSize(displayPhrase).X / 2);
                ImGui.TextColored(new Vector4(1.0f, 0.4f, 0.4f, 1.0f), displayPhrase);
                ImGui.PopFont();

                ImGui.Dummy(new Vector2(0, 10));

                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - 250);
                ImGui.SetNextItemWidth(500);
                ImGui.InputText("##AntiResellInput", ref _antiResellInput, 128);
                
                ImGui.Dummy(new Vector2(0, 15));
                
                string btnText = Lang.GetText("VerifyUnlockBtn");
                float btnWidth = Math.Max(250f, ImGui.CalcTextSize(btnText).X + 60f); 
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - btnWidth / 2);
                
                if (CustomUI.AnimatedButton("btn_verify_unlock", btnText, new Vector2(btnWidth, 40), FontAwesomeIcon.Unlock))
                {
                    if (_antiResellInput.Trim() == expectedPhrase)
                    {
                        Config.IsActivated = true; 
                        Config.Save();             
                        _antiResellInput = string.Empty; 
                    }
                    else
                    {
                        Log("Phrase mismatch!");
                    }
                }

                ImGui.Dummy(new Vector2(0, 30));
                string helpText = Lang.GetText("AntiResellHelp");
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() / 2 - ImGui.CalcTextSize(helpText).X / 2);
                ImGui.TextDisabled(helpText);

                ImGui.EndChild();
                ImGui.PopStyleColor();
                
                return; 
            }

            string version = typeof(AlmondHousing).Assembly.GetName().Version?.ToString();
            if (string.IsNullOrEmpty(version)) version = "7.5.1.0";
            if (version.EndsWith(".0")) version = version.Substring(0, version.Length - 2); 
            
            string versionText = $" v{version}";
            string authorText = " By AlmondCookie";

            float maxTextWidth = ImGui.CalcTextSize(Lang.GetText("Home")).X;maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Interior Furniture")).X);maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Fixtures")).X);maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Layout & Export")).X);maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Settings")).X);maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Material Audit")).X);maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(authorText).X);

            float sidebarWidth = maxTextWidth + 65f; 
            if (sidebarWidth < 200f) sidebarWidth = 200f; 

            ImGui.BeginChild("AlmondSidebar", new Vector2(sidebarWidth, 0), false);
            {
                DrawSidebarItem(FontAwesomeIcon.Home, Lang.GetText("Home"), 0);
                DrawSidebarItem(FontAwesomeIcon.Chair, Lang.GetText("Interior Furniture"), 1); 
                DrawSidebarItem(FontAwesomeIcon.PaintRoller, Lang.GetText("Fixtures"), 2);               
                DrawSidebarItem(FontAwesomeIcon.ClipboardList, Lang.GetText("Material Audit"), 5);
                DrawSidebarItem(FontAwesomeIcon.FileExport, Lang.GetText("Layout & Export"), 3);        
                DrawSidebarItem(FontAwesomeIcon.Cog, Lang.GetText("Settings"), 4);               

                ImGui.SetCursorPosY(Math.Max(ImGui.GetCursorPosY() + 20, ImGui.GetWindowHeight() - 60));
                ImGui.Separator();
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                ImGui.TextUnformatted(versionText);
                ImGui.TextUnformatted(authorText);
                ImGui.PopStyleColor();
            }
            ImGui.EndChild();

            ImGui.SameLine();

            ImGui.BeginChild("AlmondContent", new Vector2(0, 0), false);
            {
                switch (selectedTab)
                {
                    case 0: DrawHomeTab(); break;
                    case 1: DrawFurnitureTab(); break;
                    case 2: DrawFixtureTab(); break;
                    case 3: DrawLayoutFileTab(); break;
                    case 4: DrawSettingsTab(); break;
                    case 5: DrawMaterialTab(); break; 
                }
            }
            ImGui.EndChild();

            this.FileDialogManager.Draw();
        }

        private void DrawSidebarItem(FontAwesomeIcon icon, string label, int index)
        {
            if (!_sidebarAnimStates.ContainsKey(index)) _sidebarAnimStates[index] = 0f;

            float height = 38f;
            Vector2 startPos = ImGui.GetCursorPos(); 
            Vector2 screenPos = ImGui.GetCursorScreenPos(); 
            var drawList = ImGui.GetWindowDrawList();

            bool isSelected = selectedTab == index;
            
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0,0,0,0));
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0,0,0,0));
            if (ImGui.Selectable($"##{index}_{label}", isSelected, ImGuiSelectableFlags.None, new Vector2(0, height)))
            {
                selectedTab = index;
            }
            ImGui.PopStyleColor(2);

            bool isHovered = ImGui.IsItemHovered();
            float targetState = isSelected ? 1f : (isHovered ? 0.3f : 0f);
            float deltaTime = ImGui.GetIO().DeltaTime;
            _sidebarAnimStates[index] += (targetState - _sidebarAnimStates[index]) * (deltaTime * 15.0f);
            float progress = _sidebarAnimStates[index];

            Vector4 baseColor = new Vector4(0.55f, 0.55f, 0.55f, 1f); 
            Vector4 activeColor = ACCENT_COLOR;                       
            
            Vector4 currentColor = new Vector4(
                baseColor.X + (activeColor.X - baseColor.X) * progress,
                baseColor.Y + (activeColor.Y - baseColor.Y) * progress,
                baseColor.Z + (activeColor.Z - baseColor.Z) * progress,
                1f
            );

            float currentWidth = ImGui.GetWindowWidth();

            if (progress > 0.01f)
            {
                Vector4 bgColor = new Vector4(ACCENT_COLOR.X, ACCENT_COLOR.Y, ACCENT_COLOR.Z, progress * 0.15f);
                drawList.AddRectFilled(screenPos, screenPos + new Vector2(currentWidth, height), ImGui.ColorConvertFloat4ToU32(bgColor), 6.0f);
            }

            if (progress > 0.01f)
            {
                float lineH = height * progress * 0.6f; 
                float lineY = screenPos.Y + (height - lineH) / 2;
                drawList.AddLine(new Vector2(screenPos.X + 2, lineY), new Vector2(screenPos.X + 2, lineY + lineH), ImGui.ColorConvertFloat4ToU32(ACCENT_COLOR), 4.0f);
            }

            float xOffset = 12f + (progress * 6f);
            
            string iconStr = icon.ToIconString();
            ImGui.PushFont(UiBuilder.IconFont);
            Vector2 iconSize = ImGui.CalcTextSize(iconStr);
            drawList.AddText(new Vector2(screenPos.X + xOffset, screenPos.Y + (height - iconSize.Y) / 2), ImGui.ColorConvertFloat4ToU32(currentColor), iconStr);
            ImGui.PopFont();

            Vector2 textSize = ImGui.CalcTextSize(label);
            drawList.AddText(new Vector2(screenPos.X + xOffset + iconSize.X + 8f, screenPos.Y + (height - textSize.Y) / 2), ImGui.ColorConvertFloat4ToU32(currentColor), label);

            ImGui.SetCursorPos(new Vector2(startPos.X, startPos.Y + height + 2));
        }

        private void DrawInlineIcon(FontAwesomeIcon icon)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text(icon.ToIconString());
            ImGui.PopFont();
        }

        private void DrawInlineIconColored(FontAwesomeIcon icon, Vector4 color)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.TextColored(color, icon.ToIconString());
            ImGui.PopFont();
        }

        public static void DrawIcon(ushort icon, Vector2 size)
        {
            if (icon < 65000)
            {
                try
                {
                    var iconTexture = DalamudApi.TextureProvider.GetFromGameIcon(new GameIconLookup(icon));
                    if (iconTexture?.GetWrapOrEmpty() != null)
                    {
                        ImGui.Image(iconTexture.GetWrapOrEmpty().Handle, size);
                    }
                }
                catch (Exception)
                {
                    ImGui.Dummy(size);
                }
            }
        }
    }
}