﻿using Threax.Provision.CheapAzure.ArmTemplates.KeyVault;
using Threax.Provision.CheapAzure.HiddenResources;
using Threax.Provision.CheapAzure.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Threax.Provision;
using Threax.Provision.AzPowershell;
using Threax.Configuration.AzureKeyVault;

namespace Threax.Provision.CheapAzure.Controller.Create
{
    class CreateKeyVault : IResourceProcessor<KeyVault>
    {
        private readonly IArmTemplateManager armTemplateManager;
        private readonly Config config;
        private readonly IKeyVaultManager keyVaultManager;
        private readonly ThreaxAzureKeyVaultConfig azureKeyVaultConfig;

        public CreateKeyVault(IArmTemplateManager armTemplateManager, Config config, IKeyVaultManager keyVaultManager, ThreaxAzureKeyVaultConfig azureKeyVaultConfig)
        {
            this.armTemplateManager = armTemplateManager;
            this.config = config;
            this.keyVaultManager = keyVaultManager;
            this.azureKeyVaultConfig = azureKeyVaultConfig;
        }

        public async Task Execute(KeyVault resource)
        {
            if (!await keyVaultManager.Exists(config.InfraKeyVaultName))
            {
                var keyVaultArm = new ArmKeyVault(config.InfraKeyVaultName, config.Location, config.TenantId.ToString());
                await armTemplateManager.ResourceGroupDeployment(config.ResourceGroup, keyVaultArm);
            }

            if (!await keyVaultManager.Exists(azureKeyVaultConfig.VaultName))
            {
                var keyVaultArm = new ArmKeyVault(azureKeyVaultConfig.VaultName, config.Location, config.TenantId.ToString());
                await armTemplateManager.ResourceGroupDeployment(config.ResourceGroup, keyVaultArm);
            }
        }
    }
}
