﻿// Copyright (c) 2015 Sarin Na Wangkanai, All Rights Reserved.
// The GNU GPLv3. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.AspNetCore.Responsiveness.Abstractions.Devices;

namespace Wangkanai.AspNetCore.Responsiveness.Devices
{
    public class DefaultDevice : IDevice
    {
        public bool IsDesktop => _deviceType == DeviceType.Desktop;
        public bool IsTablet => _deviceType == DeviceType.Tablet;
        public bool IsMobile => _deviceType == DeviceType.Mobile;
        public bool IsCrawler => _deviceType == DeviceType.Crawler;

        private readonly DeviceType _deviceType;

        public DefaultDevice(DeviceType deviceType)
        {
            _deviceType = deviceType;
        }
    }
}