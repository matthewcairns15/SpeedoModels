using Braintree;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using SpeedoModels;

namespace SpeedoModels.Models
{
    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        /// <summary>
        /// Getters and setters for braintree
        /// </summary>
        Braintree.Environment Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }

        /// <summary>
        /// Creates the gateway for braintree
        /// </summary>
        /// <returns>
        /// new BraintreeGateway
        /// </returns>

        public IBraintreeGateway CreateGateway()
        {
            Environment = Braintree.Environment.SANDBOX;
            MerchantId = System.Environment.GetEnvironmentVariable("BraintreeMerchantId");
            PublicKey = System.Environment.GetEnvironmentVariable("BraintreePublicKey");
            PrivateKey = System.Environment.GetEnvironmentVariable("BraintreePrivateKey");

            if (MerchantId == null || PublicKey == null || PrivateKey == null)
            {
                Environment = Braintree.Environment.SANDBOX;
                MerchantId = "kdhkwkvvrk2gcxbv";
                PublicKey = "rjspp2vshc7bc6kz";
                PrivateKey = "af357a564d1714ebdaae2e3143c56dbe";
                
            }

            return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
        }

        /// <summary>
        /// gets the appsettings
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>
        /// ConfigurationManager.AppSettings[setting];
        /// </returns>

        public string GetConfigurationSetting(string setting)
        {
            
            return ConfigurationManager.AppSettings[setting];
        }
        /// <summary>
        /// if a BraintreeGateway doesnt exist it will call CreateGateway
        /// </summary>
        /// <returns>
        /// BraintreeGateway
        /// </returns>

        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
            
        }
        
       
        

    }
}