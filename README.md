# Setting up the Elastic Stack for Log Monitoring and Analytics

## Prerequisites

Java (required for Elastic Search only)

[Java SE Download](http://www.oracle.com/technetwork/java/javase/downloads/index.html)

Elastic Search

https://www.elastic.co/downloads/elasticsearch

Logstash

https://www.elastic.co/downloads/logstash

Kibana

https://www.elastic.co/downloads/kibana

Beats (FileBeat for log files)

https://www.elastic.co/downloads/beats/filebeat


For version 5.4.1:




```
curl -L -O https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-5.4.1.zip
unzip elasticsearch-5.4.1.zip

curl -L -O https://artifacts.elastic.co/downloads/logstash/logstash-5.4.1.zip
unzip logstash-5.4.1.zip

curl -L -O https://artifacts.elastic.co/downloads/kibana/kibana-5.4.1-windows-x86.zip
unzip kibana-5.4.1-windows-x86.zip

curl -L -O https://artifacts.elastic.co/downloads/beats/filebeat/filebeat-5.4.1-windows-x86_64.zip
unzip filebeat-5.4.1-windows-x86_64.zip
```

Create a startup script for ES and Kibana:
```
@echo off

start "ElasticSearch" elasticsearch-5.4.1\bin\elasticsearch.bat

start "Kibana" kibana-5.4.1-windows-x86\bin\kibana.bat

start "Logstash" logstash-5.4.1\bin\logstash.bat -f logstash.pipeline.conf

start "FileBeat" filebeat-5.4.1-windows-x86_64\filebeat.exe  -f logstash.pipeline.conf


```





```
input {
    tcp { 
        id => "input-tcp"
        port=>5001 
    }

    tcp { 
        id => "input-tcp2"
        port=>5002
        codec => json
    }
    
    beats { 
        id => "input-beats"
        port => 5000 }
}

filter {
    fingerprint {
        id => "fingerprint"
        method => "MD5"
        key => "key"
        target => "doc_id"
        base64encode => false
    }


    grok {
        id => "match-message"
        match => {
            "message" => "^(?<ts>2\d{3}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3,4}]) \[(?<thread>\d+)\] (?<level>\w+)\s+(?<logger>[^\s]+) \[(?<ctx>[^\]]+)\] - (?<message>.*)"
        }
        overwrite => [ "message" ]
    }

    date {
        id => "parse-timestamp"
        match => [ "ts" , "yyyy-MM-dd HH:mm:ss.SSS", "yyyy-MM-dd HH:mm:ss,SSS" ]
        remove_field => [ "ts" ]
    }


}
output {

    stdout { 
        id=> "stdout"
        codec => rubydebug { metadata => true } }
}

```














