using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    public string daid;
    public string daname;
    public string daclass;
    public string localId;
    
    public User(string id, string name, string daclass, string localid)
    {
        daid = id;
        daname = name;
        daclass = daclass;
        localId = localid;
    }
}