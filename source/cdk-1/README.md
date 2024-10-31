# Welcome to your CDK C# project!

This is a blank project for CDK development with C#.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template

# Capture1
To install the AWS Cloud Development Kit (CDK), you can follow these steps:

1. Ensure you have Node.js installed on your system. The CDK requires Node.js version 10.13.0 or later.

2. Open your terminal or command prompt.

3. Run the following command to install the AWS CDK globally using npm (Node Package Manager):

   ```
   npm install -g aws-cdk
   ```

4. After the installation is complete, you can verify the installation by checking the CDK version:

   ```
   cdk --version
   ```

   This should display the version number of the installed CDK.

5. If you plan to use TypeScript with the CDK (which is a common choice), you may also want to install TypeScript globally:

   ```
   npm install -g typescript
   ```

Once installed, you can start using the CDK to define your AWS infrastructure as code. Remember to keep your CDK installation up to date by periodically running the installation command again, as new versions are released frequently with bug fixes and new features.

For the most current installation instructions and system requirements, I recommend checking the official AWS CDK documentation.
Sources
[1] [AWS CDK tutorial for AWS Cloud9 - AWS Cloud9] (https://docs.aws.amazon.com/cloud9/latest/user-guide/sample-cdk.html)
[2] [Getting started with AWS SAM and the AWS CDK - AWS Serverless Application Model] (https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-cdk-getting-started.html)
[3] [Using an AWS AppSync API with the AWS CDK - AWS AppSync GraphQL] (https://docs.aws.amazon.com/appsync/latest/devguide/using-your-api.html)

End of Capture1

# Capture2
To use AWS CDK with C#, you can follow these steps:

1. Ensure you have the necessary prerequisites:
   - Install .NET Core SDK (version 3.1 or later)
   - Install Node.js (version 10.13.0 or later)
   - Install AWS CDK Toolkit globally: `npm install -g aws-cdk`

2. Create a new CDK project:
   - Open a terminal and navigate to your desired directory
   - Run: `cdk init app --language csharp`
   - This creates a new C# CDK project with a basic structure
Project `Cdk1\Cdk1.csproj` added to the solution.
# Welcome to your CDK C# project!

This is a blank project for CDK development with C#.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template
 All done!

3. Open the project in your preferred IDE (e.g., Visual Studio or Visual Studio Code)

4. The main CDK code will be in the `Program.cs` file. This is where you'll define your stack(s) and resources.

5. Use NuGet to install the necessary AWS CDK packages. The core package is `Amazon.CDK.Lib`, but you may need additional packages depending on the AWS services you're using.

6. Define your infrastructure as code using C# classes that inherit from `Stack`. For example:

   ```csharp
   using Amazon.CDK;
   using Amazon.CDK.AWS.S3;

   public class MyStack : Stack
   {
       public MyStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
       {
           new Bucket(this, "XXXXXXXX", new BucketProps
           {
               Versioned = true
           });
       }
   }
   ```

7. Synthesize your CDK app to generate the CloudFormation template:
   `cdk synth`

8. Deploy your stack:
   `cdk deploy`

Remember to check the official AWS CDK documentation for the most up-to-date information on using CDK with C#, as well as best practices and advanced features.
Sources
[1] [AWS CDK for .NET | AWS Developer Tools Blog] (https://aws.amazon.com/blogs/developer/aws-cdk-for-net/)
[2] [The AWS Cloud Development Kit (AWS CDK) is Now Generally Available in Java and C#] (https://aws.amazon.com/about-aws/whats-new/2019/11/the-aws-cloud-development-kit-aws-cdk-is-now-generally-available-in-java-and-c/)
[3] [Tips & Tricks: Debugging your C# CDK project in Visual Studio | AWS Developer Tools Blog] (https://aws.amazon.com/blogs/developer/tips-tricks-debugging-your-c-cdk-project-in-visual-studio/)

End of Capture2

https://docs.aws.amazon.com/cdk/v2/guide/work-with-cdk-csharp.html
https://docs.aws.amazon.com/cdk/v2/guide/cli.html
https://docs.aws.amazon.com/cdk/api/v2/

On folder for cdk.json:
cdk synth | Out-File template.yml