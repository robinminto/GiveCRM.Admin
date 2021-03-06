﻿using System;
using System.Transactions;
using System.Web.Security;
using GiveCRM.Admin.Models;

namespace GiveCRM.Admin.DataAccess
{
    public class CharityMembershipService : ICharityMembershipService
    {
        private readonly Charities CharitiesDataAccess;
        private readonly CharitiesMemberships CharitiesMembershipsDataAccess;

        public CharityMembershipService()
        {
            CharitiesDataAccess = new Charities();
            CharitiesMembershipsDataAccess = new CharitiesMemberships();
        }

        public bool RegisterUserAndCharity(RegistrationInfo registrationInfo)
        {
            bool result = false;
            var charity = new Charity
            {
                Name = registrationInfo.CharityName,
                RegisteredCharityNumber = registrationInfo.CharityName,
                SubDomain = registrationInfo.SubDomain
            };
            using (var scope = new TransactionScope())
            {
                var newCharity = CharitiesDataAccess.Insert(charity);
                if (newCharity == null) throw new ArgumentNullException("newCharity");


                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(registrationInfo.UserIdentifier, registrationInfo.Password, registrationInfo.UserIdentifier, null, null, true, null, out createStatus);
                var user = Membership.GetUser(registrationInfo.UserIdentifier);
                if (user == null) throw new ArgumentNullException("user");

                var charityMembership = new CharityMembership
                {
                    CharityId = newCharity.Id,
                    UserName = user.UserName
                };
                var newCharityMembership = CharitiesMembershipsDataAccess.Insert(charityMembership);

                if (createStatus == MembershipCreateStatus.Success && newCharityMembership != null)
                {
                    result = true;
                    scope.Complete();
                }
                
            }
            return result;
        }
    }
}
