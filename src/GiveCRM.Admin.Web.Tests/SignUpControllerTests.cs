﻿using GiveCRM.Admin.DataAccess;
using GiveCRM.Admin.Web.Controllers;
using GiveCRM.Admin.Web.Interfaces;
using GiveCRM.Admin.Web.ViewModels.SignUp;
using MvcContrib.TestHelper;
using NSubstitute;
using NUnit.Framework;
using IConfiguration = GiveCRM.Admin.Web.Interfaces.IConfiguration;

namespace GiveCRM.Admin.Web.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class SignUpControllerTests
    {
        private IConfiguration configuration;
        private ISignUpQueueingService signUpQueueingService;
        private ICharityMembershipService charityMembershipService;

        [SetUp]
        public void SetUp()
        {
            configuration = Substitute.For<IConfiguration>();
            signUpQueueingService = Substitute.For<ISignUpQueueingService>();
            charityMembershipService = Substitute.For<ICharityMembershipService>();
        }

        [Test]
        public void SignUp_RedirectsGetToHome()
        {
            var controller = new SignUpController(configuration, signUpQueueingService, charityMembershipService);
            var result = controller.SignUp();

            result.AssertActionRedirect();
        }

        [Test]
        public void SignUp_WithSuccessfulStore_RedirectsToComplete()
        {
            var requiredInfoViewModel = new RequiredInfoViewModel
                                            {
                                                UserIdentifier = "A",
                                                Password = "B",
                                                CharityName = "C",
                                            };

            var controller = new SignUpController(configuration, signUpQueueingService, charityMembershipService);
            charityMembershipService.RegisterUserAndCharity(null).ReturnsForAnyArgs(true);
            var result = controller.SignUp(requiredInfoViewModel);
            result.AssertActionRedirect().ToAction("Complete");
        }

        [Test]
        public void SignUp_WithUnsuccessfulStore_RendersView()
        {
            var requiredInfoViewModel = new RequiredInfoViewModel
                                            {
                                                UserIdentifier = "A",
                                                Password = "B",
                                                CharityName = "C",
                                            };

            var controller = new SignUpController(configuration, signUpQueueingService, charityMembershipService);
            charityMembershipService.RegisterUserAndCharity(null).ReturnsForAnyArgs(false);
            var result = controller.SignUp(requiredInfoViewModel);
            
            result.AssertViewRendered();
        }

        [Test]
        public void Complete_RendersView()
        {
            var controller = new SignUpController(configuration, signUpQueueingService, charityMembershipService);
            var result = controller.Complete();
            result.AssertViewRendered().WithViewData<CompleteViewModel>();
        }

        [Test]
        public void StoreAdditionalInfo_RendersView()
        {
            var controller = new SignUpController(configuration, signUpQueueingService, charityMembershipService);
            var result = controller.StoreAdditionalInfo(new CompleteViewModel());
            result.AssertViewRendered().WithViewData<CompleteViewModel>();
        }

        [Test]
        public void StartSite_RendersView()
        {
            var controller = new SignUpController(configuration, signUpQueueingService, charityMembershipService);
            var result = controller.StartSite("");
            result.AssertViewRendered().WithViewData<StartSiteViewModel>();
        }
    }
    // ReSharper restore InconsistentNaming
}