﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pumkin.UnityTools.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class AutoLoadAttribute : Attribute
    {
        private string parentModuleID;
        private string configurationString;
        private string id;        

        /// <summary>
        /// The ID of the module or tool, used to organize the UI
        /// </summary>
        public string ID
        {
            get => id;
            private set
            {  
                if(string.IsNullOrWhiteSpace(value))
                    throw new NullReferenceException("ID cannot be null or empty");
                id = value.ToUpperInvariant();
            }
        }

        /// <summary>
        /// The iD of the parent module, used to organize the UI
        /// </summary>
        public string ParentModuleID
        {
            get => parentModuleID;
            set => parentModuleID = string.IsNullOrWhiteSpace(value) ? null : value.ToUpperInvariant();
        }

        /// <summary>
        /// Configuration name, used to only load modules or tools if the selected configuration matches, ex: vrchat
        /// </summary>
        public string ConfigurationString
        {
            get => configurationString;
            set => configurationString = string.IsNullOrWhiteSpace(value) ? "generic" : value;
        }

        public AutoLoadAttribute(string id)
        {
            ID = id;
            ConfigurationString = default;
        }

        public static implicit operator bool(AutoLoadAttribute attr)
        {
            return !ReferenceEquals(attr, null);
        }
    }
}
