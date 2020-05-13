using System.Linq;

public class DBManager {
    public static DBManager shared() {
        if(instance == null) {
            instance = new DBManager();
        }
        return instance;
    }
    private static DBManager instance;

    private User[] users;

    public User? Login(string username, string password) {
        if (!users.Any(c => c.username == username && c.password == password)) return null;
        return users.FirstOrDefault(c => c.username == username && c.password == password);
    }
        
    private DBManager() {
        Load();
    }
    
    private void Load() {
        users = new[] {
            new User("present", "123", User.UserType.presenter),
            new User("oliver", "123", User.UserType.admin),
            new User("admin", "admin", User.UserType.admin)
        };
    }

}
