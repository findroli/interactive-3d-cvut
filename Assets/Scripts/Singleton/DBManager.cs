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
        return users.FirstOrDefault(c => c.username == username && c.password == password);
    } 
        
    private DBManager() {
        Load();
    }
    
    private void Load() {
        users = new[] {
            new User("test", "test"),
            new User("oliver", "123"),
            new User("admin", "admin")
        };
    }

}
