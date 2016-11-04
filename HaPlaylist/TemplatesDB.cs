/* Copyright (C) 2016 haha01haha01

* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using Android.Database;

namespace HaPlaylist
{
    public class TemplatesDB : SQLiteOpenHelper
    {

        private const string TABLE_NAME = "templates";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_VALUE = "value";
        private const string TABLE_CREATE =
                "CREATE TABLE " + TABLE_NAME + " (" +
                KEY_ID + " INTEGER PRIMARY KEY AUTOINCREMENT," +
                KEY_NAME + " TEXT, " +
                KEY_VALUE + " TEXT);";
        private static readonly Tuple<string, string>[] DEFAULT_TEMPLATES = { new Tuple<string, string>("<Custom Query>", ""), new Tuple<string, string>("All Music", "true") };

        public TemplatesDB(Context context) : base(context, "settings", null, 1)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL(TABLE_CREATE);
            foreach (Tuple<string, string> template in DEFAULT_TEMPLATES)
            {
                ContentValues c = new ContentValues();
                c.Put(KEY_NAME, template.Item1);
                c.Put(KEY_VALUE, template.Item2);
                db.InsertOrThrow(TABLE_NAME, null, c);
            }
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
        }

        public List<Tuple<string, string>> GetData()
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            using (ICursor c = ReadableDatabase.Query(TABLE_NAME, new string[] { KEY_ID, KEY_NAME, KEY_VALUE }, null, null, null, null, KEY_ID))
            {
                if (c != null)
                {
                    int name_idx = c.GetColumnIndexOrThrow(KEY_NAME);
                    int value_idx = c.GetColumnIndexOrThrow(KEY_VALUE);
                    while (c.MoveToNext())
                    {
                        result.Add(new Tuple<string, string>(c.GetString(name_idx), c.GetString(value_idx)));
                    }
                }
            }
            return result;
        }

        public void AddData(string name, string value)
        {
            ContentValues c = new ContentValues();
            c.Put(KEY_NAME, name);
            c.Put(KEY_VALUE, value);
            WritableDatabase.InsertOrThrow(TABLE_NAME, null, c);
        }

        public void RemoveData(string name)
        {
            WritableDatabase.Delete(TABLE_NAME, KEY_NAME + " == ?", new string[] { name });
        }
    }
}
