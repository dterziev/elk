#!/bin/sh

curl -L -O https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-6.1.2.zip
unzip elasticsearch-6.1.2.zip -d bin/

curl -L -O https://artifacts.elastic.co/downloads/logstash/logstash-6.1.2.zip
unzip logstash-6.1.2.zip -d bin/

curl -L -O https://artifacts.elastic.co/downloads/kibana/kibana-6.1.2-windows-x86_64.zip
unzip kibana-6.1.2-windows-x86_64.zip -d bin/

curl -L -O https://artifacts.elastic.co/downloads/beats/filebeat/filebeat-6.1.2-windows-x86_64.zip
unzip filebeat-6.1.2-windows-x86_64.zip -d bin/

echo '@echo off'> start-db-and-ui.cmd
echo 'start "ElasticSearch" cmd /k bin\elasticsearch-6.1.2\bin\elasticsearch.bat'>> start-db-and-ui.cmd
echo 'start "Kibana" cmd /k bin\kibana-6.1.2-windows-x86_64\bin\kibana.bat'>> start-db-and-ui.cmd

echo '@echo off' > start-logstash.cmd
echo 'start "Logstash" cmd /k bin\logstash-6.1.2\bin\logstash.bat -f config/logstash/*.conf --config.reload.automatic'>> start-logstash.cmd

echo '@echo off' > start-filebeat.cmd
echo 'start "Filebeat" cmd /k bin\filebeat-6.1.2-windows-x86_64\filebeat.exe -e -c config\filebeat\filebeat.yml'>> start-filebeat.cmd

echo '@echo off' > start-log-generator.cmd
echo 'pushd'>> start-log-generator.cmd
echo 'cd log-generator\src\LogGenerator'>> start-log-generator.cmd
echo 'dotnet restore'>> start-log-generator.cmd
echo 'dotnet build'>> start-log-generator.cmd
echo 'dotnet run'>> start-log-generator.cmd
echo 'popd'>> start-log-generator.cmd





