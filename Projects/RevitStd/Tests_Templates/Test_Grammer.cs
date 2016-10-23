
using System;

// End of VB project level imports

namespace RevitStd.Tests_Templates
{


    public class GrammerTest
    {
        private delegate void event1EventHandler(string UserName);

        private event1EventHandler event1Event;

        private event event1EventHandler event1
        {
            add { event1Event = (event1EventHandler)Delegate.Combine(event1Event, value); }
            remove { event1Event = (event1EventHandler)Delegate.Remove(event1Event, value); }
        }

        private Action<string> event2Event;

        private event Action<string> event2
        {
            add { event2Event = (Action<string>)Delegate.Combine(event2Event, value); }
            remove { event2Event = (Action<string>)Delegate.Remove(event2Event, value); }
        }

        private EventHandler<string> event3Event;

        private event EventHandler<string> event3
        {
            add { event3Event = (EventHandler<string>)Delegate.Combine(event3Event, value); }
            remove { event3Event = (EventHandler<string>)Delegate.Remove(event3Event, value); }
        }


        // Declare the delegate (if using non-generic pattern).
        public delegate void SampleEventHandler();

        // Declare the event.
        private SampleEventHandler event4Event;

        public event SampleEventHandler event4
        {
            add { event4Event = (SampleEventHandler)Delegate.Combine(event4Event, value); }
            remove { event4Event = (SampleEventHandler)Delegate.Remove(event4Event, value); }
        }


        public GrammerTest()
        {
            if (event1Event != null)
                event1Event("");
            if (event2Event != null)
                event2Event("");
            if (event3Event != null)
                event3Event(this, "");
            if (event4Event != null)
                event4Event();
        }
    }
}