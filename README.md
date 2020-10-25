# Welcome to View Renderer
This is an ASP.NET Core service that can render a view to string. The service is most commonly used by [Emailer service](https://github.com/rprimora/Emailer).
## Setup
**RazorViewToStringRenderer** - This service is responsible for rendering razor view(`*.cshtml`) to string. Service is added in the `ConfigureServices` method in following way:

    services.AddRazorViewToStringRenderer(options => 
    {
    	options.ContentRoot = HostingEnvironment.ContentRootPath;
    	options.EmailsFolder = "Emails";
    });

 - `ContentRoot` - this option is needed so the service knows where to look for views. It is obtainable via `IWebHostEnvironment` interface.
 - `EmailsFolder` - this setting is optional. Default value is `"Emails"` and this is the name of the folder that should be in folder Views.
>**Note** - The service is added as transient.
## Usage
Lets say we have a user registration model that looks like this:

    public class ActivationEmailModel
    {
    	public string ActivationLink { get; set; }
    } 

Then lets say we have a RazorView looking like this:
#### *Views/Emails/Registration.cshtml*

    @model ActivationEmailModel
    <div>
    	Thank you for registering with us please activate you account by clicking the link below
    	<a href="@Model.ActivationLink">Activate</a>
    </div>

First we would obtain the service by asking the service provider for it.

    var viewRenderer = IServiceProvider.GetService<IViewToStringRenderer>();

Then we call the method with two parameters:
- viewName - name of the view that will be converted to its string representation
- model - model that the view will use

The method call would look like following:

    var model = new ActivationEmailModel 
    {
    	ActivationLink = "yoursite.com/account/activate?id=abc123"
    };
    
    viewRenderer.RenderViewToStringAsync("Registration", model);
