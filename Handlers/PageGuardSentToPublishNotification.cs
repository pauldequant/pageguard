﻿namespace PageGuard.Handlers
{
    using Microsoft.Extensions.Logging;
    using Controllers;
    using Umbraco.Cms.Core.Events;
    using Umbraco.Cms.Core.Notifications;
    using Umbraco.Cms.Core.Security;
    using Umbraco.Cms.Infrastructure.Scoping;
    using PageGuard.Models;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Cms.Core.Mail;

    public class PageGuardSentToPublishNotification : INotificationHandler<ContentSentToPublishNotification>
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<PageGuardApiController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILocalizationService _localizationService;

        public PageGuardSentToPublishNotification(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IScopeProvider scopeProvider, ILogger<PageGuardApiController> logger, IUserService userService, IEmailSender emailSender, IUmbracoContextAccessor umbracoContextAccessor, ILocalizationService localizationService)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _scopeProvider = scopeProvider;
            _logger = logger;
            _userService = userService;
            _emailSender = emailSender;
            _localizationService = localizationService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }
        public void Handle(ContentSentToPublishNotification notification)
        {
            var currentUser = _backOfficeSecurityAccessor?.BackOfficeSecurity?.CurrentUser;

            PageGuardApiController pageGuardApiController = new PageGuardApiController(_scopeProvider, _logger, _userService, _emailSender, _umbracoContextAccessor, _localizationService);

            pageGuardApiController.PageGuardOperation(notification.Entity.Id, currentUser.Id, (int)PageGuardStatusModel.PageStatus.CheckedIn);
        }
    }
}
