﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Wangkanai.Detection;
using Xunit;

namespace Wangkanai.Responsive.Test.Core
{
    public class ResponsiveMiddlewareTests
    {
        [Fact]
        public void Ctor_RequestDelegate_ResponsiveOptions_Success()
        {
            var options = Options.Create(new ResponsiveOptions());
            var middleware = new ResponsiveMiddleware(d => Task.Factory.StartNew(() => d), options); 
        }

        [Fact]
        public void Ctor_Null_ResponsiveOptions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ResponsiveMiddleware(null, Options.Create(new ResponsiveOptions())));
        }

        [Fact]
        public void Ctor_RequestDelegate_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ResponsiveMiddleware(d => Task.Factory.StartNew(() => d), null));
        }

        [Fact]
        public async void Invoke_HttpContext_IDeviceResolver_Success()
        {
            var context = new DefaultHttpContext();
            var options = Options.Create(new ResponsiveOptions());
            var middleware = new ResponsiveMiddleware(d => Task.Factory.StartNew(() => d), options);

            await middleware.Invoke(context, new DeviceResolver());

            Assert.Same(DeviceType.Tablet.ToString(), context.GetDevice().Device);
        }

        [Fact]
        public async void Invoke_Null_IDeviceResolver_ThrowsArgumentNullException()
        {
            var options = Options.Create(new ResponsiveOptions());
            var middleware = new ResponsiveMiddleware(d => Task.Factory.StartNew(() => d), options);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await middleware.Invoke(null, new DeviceResolver()));
        }

        [Fact]
        public async void Invoke_HttpContext_Null_ThrowsNullReferenceException()
        {
            var context = new DefaultHttpContext();
            var options = Options.Create(new ResponsiveOptions());
            var middleware = new ResponsiveMiddleware(d => Task.Factory.StartNew(() => d), options);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await middleware.Invoke(context, null));
        }

        private class DeviceResolver : IDeviceResolver
        {
            public IDevice Device => new MyTablet() { Type = DeviceType.Tablet };

            public IUserAgent UserAgent => throw new NotImplementedException();
        }

        private class MyTablet : IDevice
        {
            public DeviceType Type { get; set; }

            public bool Crawler { get; set; }
        }
    }
}
