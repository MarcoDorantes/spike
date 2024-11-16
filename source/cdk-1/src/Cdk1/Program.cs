namespace Cdk1;

using System;
using System.Linq;
using System.Collections.Generic;

using Amazon.CDK;

sealed class Program
{
    public static void Main(string[] args)
    {
        StackProps properties = new()
        {
            // If you don't specify 'env', this stack will be environment-agnostic.
            // Account/Region-dependent features and context lookups will not work,
            // but a single synthesized template can be deployed anywhere.

            // Uncomment the next block to specialize this stack for the AWS Account
            // and Region that are implied by the current CLI configuration.
            /*
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
            }
            */

            // Uncomment the next block if you know exactly what Account and Region you
            // want to deploy the stack to.
            /*
            Env = new Amazon.CDK.Environment
            {
                Account = "123456789012",
                Region = "us-east-1",
            }
            */

            // For more information, see https://docs.aws.amazon.com/cdk/latest/guide/environments.html
        };
        App app = new();
        //Cdk1Stack stack = new(app, "Cdk1Stack", properties);
        Ec2Instance.Ec2InstanceStack ec2instance = new(app, "Ec2InstanceStack", properties);
        app.Synth();
    }
}