#!/bin/sh

curl -L -O https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-5.4.1.zip
unzip elasticsearch-5.4.1.zip -d bin/

curl -L -O https://artifacts.elastic.co/downloads/logstash/logstash-5.4.1.zip
unzip logstash-5.4.1.zip -d bin/

curl -L -O https://artifacts.elastic.co/downloads/kibana/kibana-5.4.1-windows-x86.zip
unzip kibana-5.4.1-windows-x86.zip -d bin/

curl -L -O https://artifacts.elastic.co/downloads/beats/filebeat/filebeat-5.4.1-windows-x86_64.zip
unzip filebeat-5.4.1-windows-x86_64.zip -d bin/

echo '@echo off'> start-db-and-ui.cmd
echo 'start "ElasticSearch" cmd /k bin\elasticsearch-5.4.1\bin\elasticsearch.bat'>> start-db-and-ui.cmd
echo 'start "Kibana" cmd /k bin\kibana-5.4.1-windows-x86\bin\kibana.bat'>> start-db-and-ui.cmd

echo '@echo off' > start-logstash.cmd
echo 'start "Logstash" cmd /k bin\logstash-5.4.1\bin\logstash.bat -f logstash.pipeline.conf --config.reload.automatic'>> start-logstash.cmd

echo '@echo off' > start-filebeat.cmd
echo 'start "Filebeat" cmd /k bin\filebeat-5.4.1-windows-x86_64\bin\filebeat.exe -e -c filebeat.yml'>> start-filebeat.cmd


