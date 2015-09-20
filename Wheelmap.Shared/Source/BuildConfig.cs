﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Model;
using Windows.Storage;

namespace Wheelmap {

    public sealed class Config {
        public string API_KEY;
        public string API_BASEURL;
        public string BUILDTYPE;
        public string HOCKEY_APP_ID;
    }

    public static class BuildConfig {

        private const string KEY_CONFIG = "Wheelmap.BuildConfig.KEY_CONFIG";

        private static Config _config;
        private static Config config {
            get {
                if (_config == null) {
                    _config = getConfig();
                }
                return _config;
            }
        }

        public static string API_KEY {
            get {
                return config.API_KEY;
            }
        }
        
        public static string API_BASEURL {
            get {
                return config.API_BASEURL;
            }
        }
        
        public static string BUILDTYPE {
            get {
                return config.BUILDTYPE;
            }
        }

        public static string HOCKEY_APP_ID {
            get {
                return config.HOCKEY_APP_ID;
            }
        }


        public static void Init(Config config) {
            setConfig(config);
            _config = config;
        }

        private static Config getConfig() {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string json = localSettings.Values[KEY_CONFIG] as string;
            if (json != null) {
                return JsonConvert.DeserializeObject<Config>(json); ;
            } else {
                return null;
            }
        }

        private static void setConfig(Config config) {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (config == null) {
                localSettings.Values[KEY_CONFIG] = null;
            }
            var json = JsonConvert.SerializeObject(config);
            localSettings.Values[KEY_CONFIG] = json;
        }
    }
}