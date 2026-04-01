using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tazkarti.Controllers;

namespace Tazkarti.Tests.Controllers.Tests
{
    public class LanguageControllerTests
    {
        private readonly LanguageController _controller;
        private readonly Mock<HttpContext> _httpContext;
        private readonly Mock<HttpResponse> _httpResponse;
        private readonly Mock<IResponseCookies> _cookies;

        public LanguageControllerTests()
        {
            _controller = new LanguageController();
            _httpContext = new Mock<HttpContext>();
            _httpResponse = new Mock<HttpResponse>();
            _cookies = new Mock<IResponseCookies>();

            _httpResponse.Setup(r => r.Cookies).Returns(_cookies.Object);
            _httpContext.Setup(c => c.Response).Returns(_httpResponse.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext.Object
            };
        }

        [Fact]
        public void ChangeLanguage_SetsCookie_AndReturnsLocalRedirect()
        {
            // Arrange
            string culture = "ar-EG";
            string returnUrl = "/Home/Index";

            // Act
            var result = _controller.ChangeLanguage(culture, returnUrl);

            // Assert
            _cookies.Verify(c => c.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                It.Is<string>(v => v.Contains(culture)),
                It.IsAny<CookieOptions>()), Times.Once);

            var redirectResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
        }
    }
}
