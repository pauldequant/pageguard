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

    public  class PageGuardPublishedNotification : INotificationHandler<ContentPublishedNotification>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<PageGuardApiController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;

        public PageGuardPublishedNotification(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _scopeProvider = scopeProvider;
            _logger = logger;
            _userService = userService;
            _emailSender = emailSender;
            _localizationService = localizationService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            var currentUser = _backOfficeSecurityAccessor?.BackOfficeSecurity?.CurrentUser;

            PageGuardApiController pageGuardApiController = new PageGuardApiController(_scopeProvider, _logger, _userService, _emailSender, _umbracoContextAccessor, _localizationService);

            foreach (var content in notification.PublishedEntities)
            {
                pageGuardApiController.PageGuardOperation(content.Id, currentUser.Id, (int)PageGuardStatusModel.PageStatus.CheckedIn);
            }
        }
    }
}
