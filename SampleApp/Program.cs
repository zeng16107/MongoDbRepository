﻿using MongoDB.Bson;
using SampleApp.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ////Use this only when you have replication enabled
            //var subscribingRepository = MemberRepository.GetSubscribingRepository();
            //subscribingRepository.SubscriptionTriggered += SubscribingRepository_SubscriptionTriggered;

            ObjectId newMemberId = ObjectId.GenerateNewId();

            using (MemberRepository memberRepository = new MemberRepository())
            {
                var member = memberRepository.CreateNew();
                member._id = newMemberId;
                member.Title = "Test Member";
                member.UserName = "username";
                member.Password = "password";
                member.Email = "test@test.com";
                memberRepository.Save();
            }

            using (MemberRepository memberRepository = new MemberRepository())
            {
                var members = memberRepository.GetAll(query => query._id >= new ObjectId(DateTime.Today, 0, 0, 0)).ToList();

                foreach (var member in members)
                {
                    member.Title += " Lastname";
                }

                memberRepository.Save();
            }

            using (MemberRepository memberRepository = new MemberRepository())
            {
                var member = memberRepository.GetById(newMemberId);

                if (member != null)
                {
                    member.Title = "Selected Member";
                    memberRepository.Save();
                }
            }

            using (MemberRepository memberRepository = new MemberRepository())
            {
                var member = memberRepository.GetSingle(query => query.Title == "Selected Member");

                if (member != null)
                {
                    member.Title = "Selected Member 2";
                    memberRepository.Save();
                }
            }

            using (MemberRepository memberRepository = new MemberRepository())
            {
                memberRepository.Delete(query => query.Title == "Selected Member 2");
                memberRepository.Save();
            }

            using (MemberRepository memberRepository = new MemberRepository())
            {
                var member = memberRepository.CreateNew();
                bool isNew = memberRepository.IsNew(member);
            }

            using (MemberRepository memberRepository = new MemberRepository())
            {
                memberRepository.GetAll();

                memberRepository.DatabaseName = "AnotherDbName";
                memberRepository.CollectionName = "AnotherCollectionName";

                memberRepository.Save();
            }

            Console.WriteLine("Job done!");
            Console.ReadLine();
        }

        private static void SubscribingRepository_SubscriptionTriggered(object sender, Member entity)
        {
            Console.WriteLine("Subscription triggered: " + entity.Title);
        }
    }
}
