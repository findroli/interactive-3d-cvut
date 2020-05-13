public struct User {
    public enum UserType {
        presenter, admin
    }
    public string username { get; }
    public string password { get; }
    public UserType userType { get; }

    public User(string username, string password, UserType userType) {
        this.username = username;
        this.password = password;
        this.userType = userType;
    }
}