public struct User {
    public enum UserType {
        presenter, admin, editor
    }
    
    public string id { get; }
    public string email { get; }
    public string password { get; }
    public UserType userType { get; }
    public string name { get; }
    public bool approved { get; }

    public User(string email, string password, UserType userType) {
        this.email = email;
        this.password = password;
        this.userType = userType;
        this.id = "";
        this.name = "";
        this.approved = true;
    }

    public User(UserDB userDb, string id, string email, string password) {
        name = userDb.name;
        approved = userDb.approved == "yes";
        userType = StringToType(userDb.type);
        this.id = id;
        this.email = email;
        this.password = password;
    }

    private static UserType StringToType(string type) {
        switch (type) {
            case "presenter":
                return UserType.presenter;
            case "admin":
                return UserType.admin;
            case "editor":
                return UserType.editor;
            default:
                return UserType.presenter;
        }
    }
}