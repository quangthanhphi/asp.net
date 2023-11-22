﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ElectroMVC.Models
{
	public class SystemSetting
	{
		[Key]
		[StringLength(50)]
		public string SettingKey { get; set; }
        [StringLength(4000)]
        public string SettingValue { get; set; }
        [StringLength(4000)]
        public string SettingDescription { get; set; }
    }
}

