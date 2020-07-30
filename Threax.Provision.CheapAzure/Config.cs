﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.Provision.CheapAzure
{
    public class Config
    {
        /// <summary>
        /// The name of the resource group to put everything in.
        /// </summary>
        public String ResourceGroup { get; set; }

        /// <summary>
        /// The azure location to use.
        /// </summary>
        public String Location { get; set; }

        /// <summary>
        /// The current TenantId
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// The current user id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The name of the sql server to create.
        /// </summary>
        public String SqlServerName { get; set; }

        /// <summary>
        /// The name of the sql database to create. This setup shares 1 db for all apps to save money.
        /// </summary>
        public String SqlDbName { get; set; }

        /// <summary>
        /// The name of the key vault to create for general infrastructure that doesn't belong to an app.
        /// </summary>
        public String InfraKeyVaultName { get; set; }

        /// <summary>
        /// The public ip of the current machine to temporarly create a sql server firewall rule for.
        /// </summary>
        public string MachineIp { get; set; }

        /// <summary>
        /// The name of the acr to create.
        /// </summary>
        public String AcrName { get; set; }

        /// <summary>
        /// The name of the app service plan to create.
        /// </summary>
        public String AppServicePlanName { get; set; }

        /// <summary>
        /// The name of the shared app insights.
        /// </summary>
        public String AppInsightsName { get; set; }

        /// <summary>
        /// The current subscription id.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// The base name of the sa secret in the key vault. Default: "sqlsrv-sa"
        /// </summary>
        public string SqlSaBaseKey { get; set; } = "sqlsrv-sa";

        /// <summary>
        /// The base name of the secret in the infra kv for the vm admin. Default: 'vm-admin'
        /// </summary>
        public string VmAdminBaseKey { get; set; } = "vm-admin";

        /// <summary>
        /// The thumbprint of the ssl certificate to use.
        /// </summary>
        public string SslCertThumb { get; set; }

        /// <summary>
        /// The guid of the Azure Devops user to set permissions for.
        /// </summary>
        public Guid? AzDoUser { get; set; }

        /// <summary>
        /// Set this to true to allow the current user to unlock key vaults. If this is false no changes 
        /// will be made and the current user must have permissions set from somewhere else. Default: true
        /// </summary>
        public bool UnlockCurrentUserInKeyVaults { get; set; } = true;
    }
}
