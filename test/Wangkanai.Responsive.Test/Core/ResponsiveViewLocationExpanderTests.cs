using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using Wangkanai.Detection;
using Xunit;

namespace Wangkanai.Responsive.Test.Core
{
    public class ResponsiveViewLocationExpanderTests
    {
        [Fact]
        public void Ctor_Default_Success()
        {
            var locationExpander = new ResponsiveViewLocationExpander();
        }

        [Fact]
        public void Ctor_ResponsiveViewLocationFormat_Success()
        {
            var locationExpander = new ResponsiveViewLocationExpander(ResponsiveViewLocationFormat.Subfolder);
        }

        [Theory]
        [InlineData((ResponsiveViewLocationFormat)int.MinValue)]
        [InlineData((ResponsiveViewLocationFormat)int.MaxValue)]
        public void Ctor_InvalidFormat_ReturnsExpected(ResponsiveViewLocationFormat format)
        {
            var viewLocations = new[]
            {
                "/Views/{1}/{0}.cshtml",
                "/Views/Shared/{0}.cshtml"
            };

            var expectedViewLocations = new[]
            {
                "/Views/{1}/{0}.Tablet.cshtml",
                "/Views/{1}/{0}.cshtml",
                "/Views/Shared/{0}.Tablet.cshtml",
                "/Views/Shared/{0}.cshtml",
            };

            var context = SetupViewLocationExpanderContext();
            var locationExpander = new ResponsiveViewLocationExpander(format);
            locationExpander.PopulateValues(context);
            var resultLocations = locationExpander.ExpandViewLocations(context, viewLocations).ToList();

            Assert.Equal(expectedViewLocations, resultLocations.ToList());
        }

        [Fact]
        public void PopulateValues_ViewLocationExpanderContext_Success()
        {
            string deviceKey = "device";
            var context = SetupViewLocationExpanderContext();
            var locationExpander = new ResponsiveViewLocationExpander();
            locationExpander.PopulateValues(context);

            Assert.NotEqual(0, context.Values.Count);
            Assert.Same(context.ActionContext.HttpContext.GetDevice().Device, context.Values[deviceKey]);
        }

        [Fact]
        public void PopulateValues_Null_ThrowsArgumentNullException()
        {
            var locationExpander = new ResponsiveViewLocationExpander();
            Assert.Throws<ArgumentNullException>(() => locationExpander.PopulateValues(null));
        }

        public static IEnumerable<object[]> ViewLocationExpanderTestData
        {
            get
            {
                yield return new object[]
                {
                    ResponsiveViewLocationFormat.Suffix,
                    new[]
                    {
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    },
                    new[]
                    {                        
                        "/Views/{1}/{0}.Tablet.cshtml",
                        "/Views/{1}/{0}.cshtml",                        
                        "/Views/Shared/{0}.Tablet.cshtml",
                        "/Views/Shared/{0}.cshtml",
                    }
                };

                yield return new object[]
                {
                    ResponsiveViewLocationFormat.Subfolder,
                    new[]
                    {
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    },
                    new[]
                    {
                        "/Views/{1}/Tablet/{0}.cshtml",
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/Tablet/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml",
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ViewLocationExpanderTestData))]
        public void ExpandViewLocations_ViewLocationExpanderContext_IEnumerable_ReturnsExpected(
            ResponsiveViewLocationFormat format,
            IEnumerable<string> viewLocations,
            IEnumerable<string> expectedViewLocations)
        {
            var context = SetupViewLocationExpanderContext();
            var locationExpander = new ResponsiveViewLocationExpander(format);
            locationExpander.PopulateValues(context);
            var resultLocations = locationExpander.ExpandViewLocations(context, viewLocations).ToList();

            Assert.Equal(expectedViewLocations, resultLocations.ToList());
        }

        [Fact]
        public void ExpandViewLocations_NoDevice_ReturnsExpected()
        {
            var context = SetupViewLocationExpanderContext();
            var viewLocations = new List<string>() { "/Views/{1}/{0}.cshtml", "/Views/Shared/{0}.cshtml" };
            var locationExpander = new ResponsiveViewLocationExpander();
            var resultLocations = locationExpander.ExpandViewLocations(context, viewLocations);

            Assert.Equal(viewLocations, resultLocations);
        }

        [Fact]
        public void ExpandViewLocations_Null_IEnumerable_ThrowsArgumentNullException()
        {
            var locationExpander = new ResponsiveViewLocationExpander();
            Assert.Throws<ArgumentNullException>(() => locationExpander.ExpandViewLocations(null, new List<string>()));
        }

        [Fact]
        public void ExpandViewLocations_ViewLocationExpanderContext_Null_ThrowsArgumentNullException()
        {
            var locationExpander = new ResponsiveViewLocationExpander();
            Assert.Throws<ArgumentNullException>(() => locationExpander.ExpandViewLocations(SetupViewLocationExpanderContext(), null));
        }

        private ViewLocationExpanderContext SetupViewLocationExpanderContext()
        {
            var context = new ViewLocationExpanderContext(new ActionContext(), "View", "Controller", "Area", null, true);
            context.Values = new Dictionary<string, string>();
            context.ActionContext.HttpContext = new DefaultHttpContext();
            context.ActionContext.HttpContext.SetDevice(new UserPerference() { Device = DeviceType.Tablet.ToString() });

            return context;
        }
    }
}
