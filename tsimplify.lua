dbdir = freeswitch.getGlobalVariable("db_dir")
local objectuuid=argv[1]
local start_date=argv[2]
local start_time=argv[3]
local ani=argv[4]
local sipip=argv[5]
local sipno=argv[6]

local dbh_recs = freeswitch.Dbh("sqlite://"..dbdir.."/sipaarecs.db")


local function record_call()
	endtimetable = os.date("*t", os.time())
	end_time = string.format("%d-%d-%d %d:%d:%d", endtimetable.year, endtimetable.month, endtimetable.day, endtimetable.hour, endtimetable.min, endtimetable.sec)

	sql = string.format("INSERT INTO TransferRecords(StartDatetime, Enddatetime, Ani, UUID, SipIP, SipNo, Flag) VALUES(\'%s\', \'%s\', \'%s\', \'%s\', \'%s\', \'%s\', 1)", start_date .. " " .. start_time, end_time, ani, objectuuid, sipip, sipno)
dbh_recs:query(sql)
	freeswitch.consoleLog("INFO", string.format("After insert.  Affected rows:%d.\n%s\n", dbh_recs:affected_rows(), sql))
end


record_call()
api = freeswitch.API();
local callstring = "bgapi uuid_simplify "..objectuuid
freeswitch.consoleLog("notice", callstring.."\n")
freeswitch.consoleLog("notice", "start:" .. start_time .. ", end:" .. end_time .. ", ani:" .. ani .. ", uuid:" .. objectuuid .. ", ip:" .. sipip .. ", sipno:" .. sipno .. "\n");
api:executeString(callstring)