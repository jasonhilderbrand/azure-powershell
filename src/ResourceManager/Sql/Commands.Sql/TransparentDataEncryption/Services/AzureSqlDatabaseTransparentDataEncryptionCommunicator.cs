﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Azure.Common.Authentication;
using Microsoft.Azure.Common.Authentication.Models;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Sql;
using Microsoft.Azure.Management.Sql.Models;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.Azure.Commands.Sql.Common;

namespace Microsoft.Azure.Commands.Sql.TransparentDataEncryption.Services
{
    /// <summary>
    /// This class is responsible for all the REST communication with the audit REST endpoints
    /// </summary>
    public class AzureSqlDatabaseTransparentDataEncryptionCommunicator
    {
        /// <summary>
        /// The Sql client to be used by this end points communicator
        /// </summary>
        private static SqlManagementClient SqlClient { get; set; }
        
        /// <summary>
        /// Gets or set the Azure subscription
        /// </summary>
        private static AzureSubscription Subscription {get ; set; }

        /// <summary>
        /// Gets or sets the Azure profile
        /// </summary>
        public AzureProfile Profile { get; set; }

        /// <summary>
        /// Creates a communicator for Azure Sql Databases TransparentDataEncryption
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="subscription"></param>
        public AzureSqlDatabaseTransparentDataEncryptionCommunicator(AzureProfile profile, AzureSubscription subscription)
        {
            Profile = profile;
            if (subscription != Subscription)
            {
                Subscription = subscription;
                SqlClient = null;
            }
        }

        /// <summary>
        /// Gets the Azure Sql Database Transparent Data Encryption
        /// </summary>
        public Management.Sql.Models.TransparentDataEncryption Get(string resourceGroupName, string serverName, string databaseName, string clientRequestId)
        {
            return GetCurrentSqlClient(clientRequestId).TransparentDataEncryption.Get(resourceGroupName, serverName, databaseName).TransparentDataEncryption;
        }

        /// <summary>
        /// Creates or updates an Azure Sql Database Transparent Data Encryption
        /// </summary>
        public Management.Sql.Models.TransparentDataEncryption CreateOrUpdate(string resourceGroupName, string serverName, string databaseName, string clientRequestId, TransparentDataEncryptionCreateOrUpdateParameters parameters)
        {
            return GetCurrentSqlClient(clientRequestId).TransparentDataEncryption.CreateOrUpdate(resourceGroupName, serverName, databaseName, parameters).TransparentDataEncryption;
        }

        /// <summary>
        /// Gets Azure Sql Database Transparent Data Encryption Activity
        /// </summary>
        public IList<Management.Sql.Models.TransparentDataEncryptionActivity> ListActivity(string resourceGroupName, string serverName, string databaseName, string clientRequestId)
        {
            return GetCurrentSqlClient(clientRequestId).TransparentDataEncryption.ListActivity(resourceGroupName, serverName, databaseName).TransparentDataEncryptionActivities;
        }

        /// <summary>
        /// Retrieve the SQL Management client for the currently selected subscription, adding the session and request
        /// id tracing headers for the current cmdlet invocation.
        /// </summary>
        /// <returns>The SQL Management client for the currently selected subscription.</returns>
        private SqlManagementClient GetCurrentSqlClient(String clientRequestId)
        {
            // Get the SQL management client for the current subscription
            if (SqlClient == null)
            {
                SqlClient = AzureSession.ClientFactory.CreateClient<SqlManagementClient>(Profile, Subscription, AzureEnvironment.Endpoint.ResourceManager);
            }
            SqlClient.HttpClient.DefaultRequestHeaders.Remove(Constants.ClientRequestIdHeaderName);
            SqlClient.HttpClient.DefaultRequestHeaders.Add(Constants.ClientRequestIdHeaderName, clientRequestId);
            return SqlClient;
        }
    }
}