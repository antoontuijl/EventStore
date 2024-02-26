https://medium.com/@no1.melman10/getting-started-with-eventstoredb-c-13411ec08713

docker run --name myeventstore -d --restart unless-stopped -p 2113:2113 -p 1113:1113 eventstore/eventstore:latest --insecure --run-projections=All --enable-external-tcp --enable-atom-pub-over-http