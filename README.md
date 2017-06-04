Utilizer
=====================

## What is Utilizer?
Utilizer creates reports that shows the utilization of rooms in a domicile.
Heres an example on a [**report**](https://github.com/kenbochr/Utilizer/blob/master/Example/Example%20on%20a%20real%20report.xlsx).


## prerequisite
A room has a PIR (Passive InfraRed sensor) and data are collected in a SQL database with a timestamp and a bit, indicating if there has been activity in the room such as:

Date                    | Activity
------------------------|---------
2017-01-01 11:02:00.000 | 1
2017-01-01 11:33:00.000 | 0

## Setup
Install [**Publish.zip**](https://github.com/kenbochr/Utilizer/blob/master/Example/Publish.zip).
Run the program and press the key "a", to edit [**Settings.xml**](https://github.com/kenbochr/Utilizer/blob/master/PirReg/Settings.xml).  
After edit, then run the program again, and a report will be created.

## Autocall
The program is written as a console application and will close it self after it has built the report. So Utilizer can be called automatically from a scheduled task such as [**Windows Task Scheduler**](https://msdn.microsoft.com/da-dk/library/windows/desktop/aa383614(v=vs.85).aspx). 

