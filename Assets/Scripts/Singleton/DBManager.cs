using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class DBManager {
    public static DBManager shared() {
        if(instance == null) {
            instance = new DBManager();
        }
        return instance;
    }
    private static DBManager instance;

    public void LoginFirebase(string email, string password, UnityAction<bool> callback) {
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted) {
                callback(false);
                return;
            }
            callback(true);
        });
    }

    public void RegisterFirebase(string email, string password, UnityAction<bool> callback) {
        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted) {
                callback(false);
                return;
            }
            callback(true);
        });
    }
        
    private DBManager() {}
}
