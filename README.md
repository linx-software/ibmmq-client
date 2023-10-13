# Background #
This is a console app to read and write text content to IBM MQ.

We require IBM MQ connectivity at a customer. At the time of writing, Linx 6 does not have an IBMMQ plugin. This project provides a way to use Linx 6 to access IBM MQ by using the CommandLine function.

# Specification #
The app uploads and downloads text content to and from an IBM MQ queue.

Usage: IBMMQClient upload|download -h host -p port -c channel -m queueManager -n queueName -i inputDir -o outputDir -a archiveDir -l logDir -u userId -w password -r cipherSpec -s certStore
- upload|download: Required first argument. Any one of the two.
- -h host: Like -h 127.0.0.1. Default is localhost.
- -p port: Like -p 1234. Default is 1414.
- -c channel: Like -c DEV.APP.SVRCONN
- -m queueManager: Like -m QM1
- -n queueName: Like -n DEV.QUEUE.1
- -i inputDir: Like -i c:\temp\mq\input. Directory where messages are read from.
- -o outputDir: Like -o c:\temp\mq\output. Directory where messages are written to.
- -a archiveDir: Like -a c:\temp\mq\archive. Directory where messages are archived.
- -l logDir: Like -l c:\temp\mq\log. Directory where log files are written to.
- -u userID: Like -u appuser
- -w password: Like -w verylongpassword
- -r cipherSpec: Like -r TLS_RSA_WITH_AES_256_CBC_SHA256
- -s certStore: Like -s *USER. Where to find the cert in the Windows certificate store. Can be *SYSTEM or *USER

## Upload ##
Upload reads files from the input directory, puts a message containing the contents of the file on the queue, and moves the file to the archive directory prepending [messageid]_ to the filename.

examples:
````
IBMMQClient.exe upload -h 127.0.0.1 -p 1414 -c DEV.APP.SVRCONN -m QM1 -n DEV.QUEUE.1 -i c:\temp\mq\input -a c:\temp\mq\archive -l c:\temp\mq\log -u app -w passw0rd
````

## Download ##
Download reads each message from the queue, writes the contents to a file named [messageid].txt in the output directory, and then removes the file from the queue.

example:
````
IBMMQClient.exe download -h 127.0.0.1 -p 1414 -c DEV.APP.SVRCONN -m QM1 -n DEV.QUEUE.1 -o c:\temp\mq\output -l c:\temp\mq\log -u app -w passw0rd
````

## Log ##
Logs are written to a file called [date].log in the log directory.

# Notes #
Documentation to get started
https://www.ibm.com/docs/en/ibm-mq/9.3?topic=applications-developing-net
https://www.ibm.com/docs/en/ibm-mq/9.3?topic=reference-mq-net-classes-interfaces

Use TRANSPORT_MQSERIES_MANAGED mode. For other modes the MQSeries client install is required.

I tested this by running IBM MQ in a container: https://developer.ibm.com/tutorials/mq-connect-app-queue-manager-containers/
````
docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --publish 1414:1414 --publish 9443:9443 --detach --env MQ_APP_PASSWORD=passw0rd --name QM1 icr.io/ibm-messaging/mq:latest
````

I couldn't get the ibmmq console running in the container at https://localhost:9443/ibmmq/console to work, but the client and this app does work with the parameters shown in the examples above.
