# Background #
This is a console app to read and write text content to IBM MQ.

We require IBM MQ connectivity at a customer. At the time of writing, Linx 6 does not have an IBMMQ plugin. This project provides a way to use Linx 6 to access IBM MQ by using the CommandLine function.

# Specification #
The app uploads and downloads text content to and from an IBM MQ queue.

app.exe upload|download -host [host] -port [port] -channel [channel] -queuemanager [queuemanager] -userid [userid] -password [password] -queuename [queuename] -inputdir [inputdirectory] -archivedir [archivedirectory] -outputdir [outputdirectory] -logdir [logdirectory]

## Upload ##
Upload reads files from the input directory, puts a message containing the contents of the file on the queue, and moves the file to the archive directory prepending [messageid]_ to the filename.

example:
````
app.exe upload -host 127.0.0.1 -port 1414 -channel DEV.APP.SVRCONN -queuemanager QM1 -userid app -password passw0rd -queuename DEV.QUEUE.1 -inputdir c:\temp\mq\input -archivedir c:\temp\mq\archive -logdir c:\temp\mq\log
````

## Download ##
Download reads each message from the queue, writes the contents to a file named [messageid].txt in the output directory, and then removes the file from the queue.

example:
````
app.exe download -host 127.0.0.1 -port 1414 -channel DEV.APP.SVRCONN -queuemanager QM1 -userid app -password passw0rd -queuename DEV.QUEUE.1 -outputdir c:\temp\mq\output -logdir c:\temp\mq\log
````

## Log ##
Logs are written to a file called [date].log in the log directory.

# Notes #
Documentation to get started
https://www.ibm.com/docs/en/ibm-mq/9.3?topic=applications-developing-net

Use TRANSPORT_MQSERIES_MANAGED mode. For other modes the MQSeries client install is required.

I tested this by running IBM MQ in a container: https://developer.ibm.com/tutorials/mq-connect-app-queue-manager-containers/
````
docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --publish 1414:1414 --publish 9443:9443 --detach --env MQ_APP_PASSWORD=passw0rd --name QM1 icr.io/ibm-messaging/mq:latest
````

I couldn't get the ibmmq console running in the container at https://localhost:9443/ibmmq/console to work, but the client and this app does work with the parameters shown in the examples above.
