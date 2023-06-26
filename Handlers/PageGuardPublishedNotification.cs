namespace PageGuard.Handlers
{
    using Umbraco.Cms.Core.Security;
    using Umbraco.Cms.Core.Events;
    using Umbraco.Cms.Core.Notifications;
    using Controllers;
    using Models;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Infrastructure.Scoping;
    using Umbraco.Cms.Core.Mail;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Core.Web;
    using Microsoft.Extensions.Configuration;

    public class PageGuardPublishedNotification : INotificationHandler<ContentPublishedNotification>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<PageGuardApiController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly IConfiguration _configuration;

        public PageGuardPublishedNotification(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService, ILocalizedTextService localizedTextService, IConfiguration configuration)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _scopeProvider = scopeProvider;
            _logger = logger;
            _userService = userService;
            _emailSender = emailSender;
            _localizationService = localizationService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _localizedTextService = localizedTextService;
            _configuration = configuration;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            var currentUser = _backOfficeSecurityAccessor?.BackOfficeSecurity?.CurrentUser;

            PageGuardApiController pageGuardApiController = new PageGuardApiController(_scopeProvider, _logger, _userService, _emailSender, _umbracoContextAccessor, _localizationService, _localizedTextService, _configuration);

            foreach (var content in notification.PublishedEntities)
            {
                pageGuardApiController.PageGuardOperation(content.Id, currentUser.Id, (int)PageGuardStatusModel.PageStatus.CheckedIn);
            }
        }
    }
}
