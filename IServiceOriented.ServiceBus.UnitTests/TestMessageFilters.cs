﻿using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using IServiceOriented.ServiceBus.Dispatchers;

namespace IServiceOriented.ServiceBus.UnitTests
{
    [TestFixture]
    public class TestMessageFilters
    {
        public TestMessageFilters()
        {
        }

        class C1
        {
        }

        class C1IsMyBase : C1
        {
        }
        
        [Test]
        public void TypedMessageFilter_Includes_Proper_Types_Without_Inheritance()
        {
            TypedMessageFilter tmf = new TypedMessageFilter(false, typeof(int), typeof(string), typeof(C1));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, 1)));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, "test")));
            Assert.IsFalse(tmf.Include(new PublishRequest(null, null, 1M)));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, new C1())));
            Assert.IsFalse(tmf.Include(new PublishRequest(null, null, new C1IsMyBase())));
        }

        [Test]
        public void TypedMessageFilter_Includes_Proper_Types_With_Inheritance()
        {
            TypedMessageFilter tmf = new TypedMessageFilter(true, typeof(int), typeof(string), typeof(C1));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, 1)));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, "test")));
            Assert.IsFalse(tmf.Include(new PublishRequest(null, null, 1M)));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, new C1())));
            Assert.IsTrue(tmf.Include(new PublishRequest(null, null, new C1IsMyBase())));
        }


        
        [Test]
        public void UnhandledMessageFilter_Receives_Unhandled_Messages()
        {
            int handledCount = 0;
            int unhandledCount = 0;

            using (var runtime = Create.BinaryMsmqRuntime())
            {
                runtime.MessageDeliveryExpired += (o, msg) => System.Diagnostics.Trace.WriteLine("expired");

                runtime.UnhandledException += (ex, p) => System.Diagnostics.Trace.WriteLine("Unhandled Exception = "+ex);
                AutoResetEvent reset = new AutoResetEvent(false);

                SubscriptionEndpoint handled = new SubscriptionEndpoint(Guid.NewGuid(), "Handled", null, null, typeof(void), new ActionDispatcher((e, d) => { 
                    Interlocked.Increment(ref handledCount); 
                    System.Diagnostics.Trace.WriteLine("Handled Message = " + d.Message); 
                    reset.Set(); 
                }), null);
                SubscriptionEndpoint unhandled = new SubscriptionEndpoint(Guid.NewGuid(), "Unhandled", null, null, typeof(void), new ActionDispatcher((e, d) => { 
                    Interlocked.Increment(ref unhandledCount); 
                    System.Diagnostics.Trace.WriteLine("Unhandled Message = " + d.Message); 
                    reset.Set(); }), 
                    new UnhandledMessageFilter(true, typeof(object)));

                runtime.Subscribe(handled);
                runtime.Subscribe(unhandled);

                runtime.Start();

                runtime.PublishOneWay(null, null, "Handled");

                // Make sure that unhandled doesn't get the message if it is handled
                if (reset.WaitOne(1000 * 10, true))
                {
                    Assert.AreEqual(1, handledCount);
                    Assert.AreEqual(0, unhandledCount);
                }
                else
                {
                    throw new InvalidOperationException("Waited too long");
                }

                runtime.RemoveSubscription(handled);

                handledCount = 0;
                unhandledCount = 0;


                runtime.PublishOneWay(null, null, "Unhandled");

                // Make sure that unhandled gets the message if it is handled
                if (reset.WaitOne(1000 * 10, true))
                {
                    Assert.AreEqual(0, handledCount);
                    Assert.AreEqual(1, unhandledCount);
                }
                else
                {
                    throw new InvalidOperationException("Waited too long");
                }

                runtime.Stop();
            }
        }
    }
}
