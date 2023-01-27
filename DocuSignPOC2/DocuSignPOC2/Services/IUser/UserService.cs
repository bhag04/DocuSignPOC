﻿using DocuSign.Admin.Api;
using DocuSign.Admin.Model;
using DocuSign.Constants;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSignPOC2.Services.IDocuSignClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocuSign.eSign.Client;
using DocuSignPOC2.Services.IESignAdmin;

namespace DocuSignPOC2.Services.IUser
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly IeSignAdminService _iESignAdminService;
        private readonly ApiClient _apiClient;
        private readonly string _eSignAccoutId;
        private readonly string _eSignGroupId;

        public UserService(IConfiguration config, IeSignAdminService iESignAdminService)
        {
            _config = config;
            _iESignAdminService = iESignAdminService;
            _apiClient = _iESignAdminService.ApiClient;
            _eSignAccoutId = iESignAdminService.ESignAdminAccountId;


        }
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="firstName">The first name of a new user</param>
        /// <param name="lastName">The last name of a new user</param>
        /// <param name="userName">The username of a new user</param>
        /// <param name="email">The email of a new user</param>
        /// <param name="permissionProfileId">The permission profile ID that will be used for a new user</param>
        /// <param name="groupId">The group ID that will be used for a new user</param>
        /// <returns>The response of creating a new user</returns>

        public NewUsersSummary CreateNewUser(
            string firstName,
            string lastName,
            string userName,
            string email,
            long permissionProfileId)
        {

            var usersApi = new DocuSign.eSign.Api.UsersApi(_apiClient);
            var newUsersDefinition = new NewUsersDefinition { NewUsers = new List<UserInformation>() };
            UserInformation user1 = new UserInformation
            {
                FirstName = firstName,
                LastName = lastName,
                PermissionProfileId = permissionProfileId.ToString(),
                //DefaultAccountId =  accountId.ToString(),
                //GroupList = new List<Group>() { new Group() { GroupId = groupId.ToString() } },
                Email = email,
                UserName = userName,
                ActivationAccessCode = "123456"
            };
            //UserInformation user2 = new UserInformation { Email = "sam@example.com", UserName = "Sam Two", Company = "XYZ", ActivationAccessCode = "123456" };
            newUsersDefinition.NewUsers.Add(user1);
            //newUsersDefinition.NewUsers.Add(user2);
            NewUsersSummary newUsersSummary = usersApi.Create(_iESignAdminService.ESignAdminOrganizationId, newUsersDefinition);
            return newUsersSummary;
        }


        /// <summary>
        /// Gets the DocuSign permission profiles and groups
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <returns>The tuple with DocuSign permission profiles and groups information</returns>
        public (PermissionProfileInformation, GroupInformation) GetPermissionProfilesAndGroups()
        {
            var accountsApi = new DocuSign.eSign.Api.AccountsApi(_apiClient);
            var permissionProfiles = accountsApi.ListPermissions(_eSignAccoutId);

            var dsGroupsApi = new GroupsApi(_apiClient);
            var groups = dsGroupsApi.ListGroups(_eSignAccoutId);

            return (permissionProfiles, groups);
        }

        /// <summary>
        /// Constructs a request for creating a new user
        /// </summary>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="firstName">The first name of a new user</param>
        /// <param name="lastName">The last name of a new user</param>
        /// <param name="userName">The username of a new user</param>
        /// <param name="email">The email of a new user</param>
        /// <param name="permissionProfileId">The permission profile ID that will be used for a new user</param>
        /// <param name="groupId">The group ID that will be used for a new user</param>
        /// <returns>The request for creating a new user</returns>
        public NewUserRequest ConstructNewUserRequest(
            long permissionProfileId,
            long groupId,
            string email,
            string firstName,
            string lastName,
            string userName)
        {
            return new NewUserRequest
            {
                // Step 3 start
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Accounts = new List<NewUserRequestAccountProperties>
                {
                    new NewUserRequestAccountProperties
                    {
                        Id = new Guid(_eSignAccoutId),
                        PermissionProfile = new PermissionProfileRequest
                        {
                            Id = permissionProfileId,
                        },
                        Groups = new List<GroupRequest>
                        {
                            new GroupRequest
                            {
                                Id = groupId,
                            },
                        },
                    },
                },
                AutoActivateMemberships = true,

                // Step 3 end
            };
        }
    }
}
