﻿using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Content;
using Microsoft.Xna.Framework;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Manlaan.HPGrid.Models;
using Manlaan.HPGrid.Controls;

namespace Manlaan.HPGrid
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {

        private static readonly Logger Logger = Logger.GetLogger<Module>();
        internal static Module ModuleInstance;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        private SettingEntry<bool> _settingHPGridShowDesc; 
        private List<Grid> _hpgrid_items;
        private DirectoryReader _directoryReader;
        private JsonSerializerOptions _jsonOptions;
        private DrawGrid _gridImg;

        private string[] _jsonfiles = { 
            "sample.txt",
            //"Fractal_Aetherblade.json",  //None
            "Fractal_AquaticRuins.json",
            "Fractal_CaptainMaiTrin.json",  
            "Fractal_Chaos.json",
            "Fractal_Cliffside.json",  
            "Fractal_Deepstone.json",
            "Fractal_MoltenBoss.json",
            "Fractal_MoltenFurnace.json",
            "Fractal_Nightmare.json",
            "Fractal_Shattered.json",
            "Fractal_SirensReef.json",
            "Fractal_Snowblind.json",
            //"Fractal_SolidOcean.json",  //None   
            "Fractal_Sunqua.json",
            "Fractal_Swampland.json",
            "Fractal_Thaumanova.json",
            "Fractal_TwilightOasis.json", 
            //"Fractal_Uncategorized.json",  //None
            //"Fractal_Underground.json",  //None
            //"Fractal_UrbanBattlegrounds.json",  //None
            "Fractal_Volcanic.json",
            "Raid_BastionPenitent.json",
            "Raid_HallChains.json",
            "Raid_KeyAhdashim.json",
            "Raid_MythwrightGambit.json",
            "Raid_SalvationPass.json",
            "Raid_SpiritVale.json",
            "Raid_StrongholdFaithful.json",
            "Strike_Boneskinner.json",
            "Strike_FraenerJormag.json",
            "Strike_ShiverpeakPass.json",
            "Strike_WhisperJormag.json",
        };

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { ModuleInstance = this; }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingHPGridShowDesc = settings.DefineSetting("HPGridShowDesc", true, "Show Description", "");
            _settingHPGridShowDesc.SettingChanged += UpdateShowDesc;
        }

        protected override void Initialize()
        {
            _hpgrid_items = new List<Grid>();
            _gridImg = new DrawGrid();
            _gridImg.Parent = GameService.Graphics.SpriteScreen;
            _gridImg.ShowDesc = _settingHPGridShowDesc.Value;
        }

        protected override async Task LoadAsync()
        {
            foreach (string s in _jsonfiles)
            {
                ExtractFile(s);
            }

            string hpgridDirectory = DirectoriesManager.GetFullDirectoryPath("hpgrid");
            _directoryReader = new DirectoryReader(hpgridDirectory);

            _jsonOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                IgnoreNullValues = true
            };

            _directoryReader.LoadOnFileType((Stream fileStream, IDataReader dataReader) => {
                readJson(fileStream);
            }, ".json");
        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            base.OnModuleLoaded(e);
        }
        private void UpdateShowDesc(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            _gridImg.ShowDesc = _settingHPGridShowDesc.Value;
        }

        protected override void Update(GameTime gameTime)
        {
            _gridImg.Visible = false;
            
            if (GameService.GameIntegration.Gw2Instance.IsInGame && !GameService.Gw2Mumble.UI.IsMapOpen)
            {
                foreach (Grid _grid in _hpgrid_items)
                {
                    if (_grid.Map == GameService.Gw2Mumble.CurrentMap.Id)
                    {
                        foreach (GridFight _fight in _grid.Fights)
                        {
                            if (_fight.InRadius(GameService.Gw2Mumble.PlayerCharacter.Position))
                            {
                                _gridImg.SetSize();
                                _gridImg.Visible = true;
                                _gridImg.Phases = _fight.Phase;
                            }
                        }
                    }
                }
            }
        }
        /// <inheritdoc />
        protected override void Unload()
        {
            ModuleInstance = null;
            _gridImg.Dispose();
        }

        private void readJson(Stream fileStream)
        {
            string jsonContent;
            using (var jsonReader = new StreamReader(fileStream))
            {
                jsonContent = jsonReader.ReadToEnd();
            }

            Grid g = null; 
            try
            {
                g = JsonSerializer.Deserialize<Grid>(jsonContent, _jsonOptions);
                _hpgrid_items.Add(g);
            }
            catch (Exception ex)
            {
                Logger.Error("HPGrid deserialization failure: " + ex.Message);
            }
        }

        private void ExtractFile(string filePath)
        {
            var fullPath = Path.Combine(DirectoriesManager.GetFullDirectoryPath("hpgrid"), filePath);
            //if (File.Exists(fullPath)) return;
            using (var fs = ContentsManager.GetFileStream(filePath))
            {
                fs.Position = 0;
                byte[] buffer = new byte[fs.Length];
                var content = fs.Read(buffer, 0, (int)fs.Length);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                File.WriteAllBytes(fullPath, buffer);
            }
        }


    }

}
