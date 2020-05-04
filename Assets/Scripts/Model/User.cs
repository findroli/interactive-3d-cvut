public struct User {
    public string username { get; }
    public string password { get; }

    public User(string username, string password) {
        this.username = username;
        this.password = password;
    }
}