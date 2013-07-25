﻿using BrewersBuddy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BrewersBuddy.Services
{
    public class BatchNoteService : IBatchNoteService
    {
        private BrewersBuddyContext db = new BrewersBuddyContext();
        private BrewersBuddyContext db2 = new BrewersBuddyContext();

        public void Create(BatchNote @object)
        {
            db.BatchNotes.Add(@object);
            db.SaveChanges();
        }

        public void Delete(BatchNote @object)
        {
            db.BatchNotes.Remove(@object);
            db.SaveChanges();
        }

        public BatchNote Get(int id)
        {
            return db.BatchNotes.Find(id);
        }

        public IEnumerable<BatchNote> GetAllForBatch(int batchId)
        {
            return db.BatchNotes.Where(note => note.BatchId == batchId);
        }

        public void Update(BatchNote @object)
        {
            db2.Entry(@object).State = EntityState.Modified;
            db2.SaveChanges();
        }

        public void Dispose()
        {
            if (db != null)
                db.Dispose();
        }
    }
}