[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private static string ConnectionString = "Server=localhost;Database=MyDb;User Id=sa;Password=HardcodedPass!"; 

    private readonly UserService _userService = new UserService(); 
  
    [HttpPost("create")]
    public IActionResult CreateUser()
    {
        var username = Request.Form["username"]; 

        _userService.CreateUser(username);

        return Ok("User created");
    }    
}

public class User
{
	public short Id { get; set; }
	public object Username { get; set; }
	public object Email { get; set; }
}

public class UserService 
{
	private readonly EmailService _emailService = new EmailService();
	
    public async Task CreateUser(string username)
    {
         var connection = new SqlConnection(ConnectionString); 
         var command = new SqlCommand($"INSERT INTO Users (Username) VALUES ('{username}')", connection); 
         connection.Open();
         command.ExecuteNonQuery(); 
    }

    public void CreateUsers(List<User> users)
    {
        var context = new AppDbContext(); 
        foreach (var user in users)
        {
            context.Users.Add(user);
            context.SaveChanges(); 
        }
    }
    
	public void CreateOrder(long userId, Order order)
	{
		var contex = new AppDbContext();
		var user = contex.User.Find(userId);
		order.UserId = userId;
		context.Order.Add(order);
		_emailService.Send(user.Email, "Order created");
	}
	
	public User GetUserByEmail(string email)
	{
		var contex = new AppDbContext(); 
		var user = contex.User.Find(x=>x.Email = email);
		return user;
	}
}

public class EmailService
{	
	private readonly UserService _userService = new UserService();
	
	public void Send(string emailTo, string subject, string body)
	{
		var user = _userService.GetUserByEmail(email);
		if (user == null) return; 
		var fromAddress = new MailAddress("your@email.com", "Your Name");
        var toAddress = new MailAddress(emailTo);
        const string fromPassword = "your_password"; 
		
        var smtp = new SmtpClient
        {
            Host = "smtp.yourserver.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
            Timeout = 20000
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };

        smtp.Send(message);
	}	
}