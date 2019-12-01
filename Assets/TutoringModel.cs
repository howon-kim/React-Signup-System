using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutoringModel
{
    
    public User datutor;
    public User datutee;
    public long dadate;
    public Boolean daattend;
    public string tutoringdb;
    
    public TutoringModel(User tutor, User tutee, long date, Boolean attend, string dbinfo) {
        datutor = tutor;
        datutee = tutee;
        dadate = date;
        daattend = attend;
        tutoringdb = dbinfo;
    }
}