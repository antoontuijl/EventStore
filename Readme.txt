https://www.ahmetkucukoglu.com/en/event-sourcing-with-asp-net-core-01-store
https://medium.com/@no1.melman10/getting-started-with-eventstoredb-c-13411ec08713

EventSourcingTrackingApp docker run: docker run -d --name eventstore -p 2113:2113 -p 1113:1113 eventstore/eventstore --enable-external-tcp --enable-atom-pub-over-http
MyEventStore docker: docker run --name myeventstore -d --restart unless-stopped -p 2113:2113 -p 1113:1113 eventstore/eventstore:latest --insecure --run-projections=All --enable-external-tcp --enable-atom-pub-over-http


http://localhost:2113/web/index.html#/streams

host.docker.internal