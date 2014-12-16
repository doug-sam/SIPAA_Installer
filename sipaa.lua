dbdir = freeswitch.getGlobalVariable("db_dir")
sipsvrip=session:getVariable("network_addr")
sipsvrport=session:getVariable("sip_network_port")
ani = session:getVariable("caller_id_number")
dnis = session:getVariable("destination_number")
objectuuid = session:get_uuid()


local dbh_cfg = freeswitch.Dbh("sqlite://"..dbdir.."/sipaacfg.db")
local dbh_recs = freeswitch.Dbh("sqlite://"..dbdir.."/sipaarecs.db")

-- variables
this_call_is_valid = false
SipNumber = {server_id = 0, sip_number_id = 0, voice_type = 0, voice_id = 0, transfer_no = "", play_times = 0, timeout_action = 1, default_no = "", no_answer_timeout = 40}
dial_rules = {}
userdefined_welcome_file = ""
system_welcome_file = ""
welcome_file = ""
system_busy_file = ""
system_fail_file = ""
system_timeout_second = 0
system_busy_times_str = ""
system_fail_times_str = ""
system_busy_times = 1
system_fail_times = 1

transfer_record = {start_time_str = "", start_date_str = "", start_hhmmss_str = "", end_time_str = "", ani = "", uuid = "", sipip = "", sipno = "", flag = 0 }
-- variables end


local function select_sipnumber(row)
	for key, val in pairs(row) do
		freeswitch.consoleLog("INFO", "field name:"..key.."\n")
		this_call_is_valid = true
			if ( key == "ID" ) then
				SipNumber.server_id = val
			elseif ( key == "SipNumberID" ) then
				SipNumber.sip_number_id = val
			elseif ( key == "VoiceType" ) then
				SipNumber.voice_type = val
			elseif ( key == "VoiceID" ) then
				if ( val == "" or val == nil ) then
					SipNumber.voice_id = 0
				else
					SipNumber.voice_id = val
				end
				freeswitch.consoleLog("INFO", "voice_id:"..val.."\n")
			elseif ( key == "TransferNo" ) then
				SipNumber.transfer_no = val
				freeswitch.consoleLog("INFO", "transfer_no:"..val.."\n")
			elseif ( key == "PlayTimes" ) then
				SipNumber.play_times = val
			elseif ( key == "TimeoutAction" ) then
				SipNumber.timeout_action = val
			elseif ( key == "DefaultNo" ) then
				if ( val == "" or val == nil ) then
					SipNumber.default_no = ""
				else
					SipNumber.default_no = val
				end
			elseif ( key == "NoAnswerTimeout" ) then
				SipNumber.no_answer_timeout = val
			end
	end
end

local function select_dialrules(row)
	local ruleobj = {key="", value=""}
	for key, val in pairs(row) do
		if ( key == "InputNo" ) then
			ruleobj.key = val
		elseif ( key == "RuleString" ) then
			ruleobj.value = val
		end
	end
	if ( ruleobj.key ~= "" and ruleobj.value ~= "" ) then
		table.insert(dial_rules, ruleobj)
	end
end

local function select_userdefined_welcome_filepath(row)
	for key, val in pairs(row) do
		userdefined_welcome_file = val
	end
end

local function select_system_filepath(row)
	local fieldname = ""
	local option_val = ""
	for key, val in pairs(row) do
		freeswitch.consoleLog("INFO", "key:" .. key .. ".  val:" .. val .. ".    option_val" .. option_val .. ".  fieldname:" .. fieldname .. "\n")
		if ( key == "OptionText" and not (option_val == "")) then
			if ( val == "DEFAULT_HELLO" ) then
				system_welcome_file = option_val
			elseif ( val == "DEFAULT_BUSY" ) then
				system_busy_file = option_val
			elseif ( val == "DEFAULT_FAIL" ) then
				system_fail_file = option_val
			elseif ( val == "DEFAULT_FAIL_TIMES" ) then
				system_fail_times_str = option_val
			elseif ( val == "DEFAULT_BUSY_TIMES" ) then
				system_busy_times_str = option_val
			end
			option_val = ""
		elseif ( key == "OptionText" ) then
			fieldname = val
		end
		if ( key == "OptionValue" ) then
			if ( fieldname == "DEFAULT_HELLO" ) then
				system_welcome_file = val
			elseif ( fieldname == "DEFAULT_BUSY" ) then
				system_busy_file = val
			elseif ( fieldname == "DEFAULT_FAIL" ) then
				system_fail_file = val
			elseif ( fieldname == "DEFAULT_BUSY_TIMES" ) then
				system_busy_times_str = val
			elseif ( fieldname == "DEFAULT_FAIL_TIMES" ) then
				system_fail_times_str = val
			elseif ( fieldname == "" ) then
				option_val = val
			end
		end
	end
end

local function select_system_timeout_second(row)
	for key, val in pairs(row) do
		system_timeout_second = val
	end
end

local function this_incomming_call_is_valid(ip, sipno)
	local sql_query = string.format("SELECT SipServerInfo.ID, SipNumber.ID AS SipNumberID, SipNumber.VoiceType, SipNumber.VoiceID, SipNumber.TransferNo, SipNumber.PlayTimes, SipNumber.TimeoutAction, SipNumber.DefaultNo, SipNumber.NoAnswerTimeout from SipServerInfo, SipNumber WHERE SipServerInfo.ID = SipNumber.ServerID AND SipServerInfo.ServerIP = \'%s\' AND SipNumber.SipNo = \'%s\' AND SipNumber.Enabled = 1", ip, sipno)
	dbh_cfg:query(sql_query, select_sipnumber)
	local logstr = string.format("ID:%d,  SipNumberID:%d,  VoiceType:%d,  VoiceID:%d,  TransferNo:%s,  PlayTimes:%d,  TimeoutAction:%d,  DefaultNo:%s,  NoAnswerTimeout:%d", SipNumber.server_id, SipNumber.sip_number_id, SipNumber.voice_type, SipNumber.voice_id, SipNumber.transfer_no, SipNumber.play_times, SipNumber.timeout_action, SipNumber.default_no, SipNumber.no_answer_timeout)
	freeswitch.consoleLog("INFO", logstr.."\n")
	return this_call_is_valid
end

local function get_dialrules_by_sipnumber(sipno)
	local sql_query = string.format("SELECT InputNo, RuleString, OrderId from SipNumber, Dialrules WHERE (SipNumber.ID = Dialrules.SipID AND SipNumber.SipNo = \'%s\') ORDER BY OrderId", sipno)
	dbh_cfg:query(sql_query, select_dialrules)
	for i,n in ipairs(dial_rules) do
		freeswitch.consoleLog("INFO", "SipNumber:" .. sipno .. "  key:" .. n.key .. ",    value:" .. n.value .. "\n")
	end
end

local function get_userdefined_welcome_file(sipno)
	local sql_query = string.format("SELECT VoicePath from SipNumber, SysVoice WHERE SipNumber.VoiceID = SysVoice.ID AND SipNumber.SipNo = \'%s\'", sipno)
	dbh_cfg:query(sql_query, select_userdefined_welcome_filepath)
	userdefined_welcome_file = "sipaapangling\\" .. userdefined_welcome_file
	freeswitch.consoleLog("INFO", "SipNumber:" .. sipno .. ",  user defined welcome file:" .. userdefined_welcome_file .. "\n")
end

local function get_system_welcome_file()
	local sql_query = string.format("SELECT OptionText, OptionValue from OptionText = \'DEFAULT_HELLO\'")
	dbh_cfg:query(sql_query, select_system_filepath)
	freeswitch.consoleLog("INFO", "System welcome file:" .. system_welcome_file .. "\n")
end

local function get_system_files()
	local sql_query = string.format("SELECT OptionText, OptionValue from SysOptions WHERE OptionText = \'DEFAULT_FAIL\' or OptionText = \'DEFAULT_BUSY\' or OptionText = \'DEFAULT_HELLO\' or OptionText = \'DEFAULT_BUSY_TIMES\' or OptionText = \'DEFAULT_FAIL_TIMES\'")
	dbh_cfg:query(sql_query, select_system_filepath)
	system_welcome_file = "sipaapangling\\" .. system_welcome_file
	system_fail_file = "sipaapangling\\" .. system_fail_file
	system_busy_file = "sipaapangling\\" .. system_busy_file
	freeswitch.consoleLog("INFO", "System welcome file:" .. system_welcome_file .. ".  fail file:" .. system_fail_file .. ".  busy file:" .. system_busy_file .. "\n")
end

local function get_system_timeout_second()
	local sql_query = string.format("SELECT OptionValue from SysOptions WHERE OptionText = \'TIMEOUT_SECOND\'")
	dbh_cfg:query(sql_query, select_system_timeout_second)
	freeswitch.consoleLog("INFO", "System timeout second:" .. system_timeout_second .. "\n")
end

local function record_call(flag)
	transfer_record.flag = tonumber(flag)
	endtimetable = os.date("*t", os.time())
	transfer_record.end_time_str = string.format("%4d-%02d-%02d %02d:%02d:%02d.000", endtimetable.year, endtimetable.month, endtimetable.day, endtimetable.hour, endtimetable.min, endtimetable.sec)


sql = string.format("INSERT INTO TransferRecords(StartDatetime, Enddatetime, Ani, UUID, SipIP, SipNo, Flag) VALUES(\'%s\', \'%s\', \'%s\', \'%s\', \'%s\', \'%s\', %d)", transfer_record.start_time_str, transfer_record.end_time_str, transfer_record.ani, transfer_record.uuid, transfer_record.sipip, transfer_record.sipno, tonumber(transfer_record.flag))
dbh_recs:query(sql)
freeswitch.consoleLog("INFO", string.format("After insert.  Affected rows:%d.\n%s\n", dbh_recs:affected_rows(), sql))
end


transfer_record.ani = ani
transfer_record.uuid = objectuuid
transfer_record.sipip = sipsvrip
transfer_record.sipno = dnis
timetable = os.date("*t", os.time())
transfer_record.start_date_str =string.format("%4d-%02d-%02d", timetable.year, timetable.month, timetable.day)
transfer_record.start_hhmmss_str = string.format("%02d:%02d:%02d.000", timetable.hour, timetable.min, timetable.sec)
transfer_record.start_time_str = string.format("%4d-%02d-%02d %02d:%02d:%02d.000", timetable.year, timetable.month, timetable.day, timetable.hour, timetable.min, timetable.sec)

if ( this_incomming_call_is_valid(sipsvrip, dnis) == false ) then
	-- cannot find any records in SipNumber and SipServerInfo table, so hangup the call.
	freeswitch.consoleLog("INFO", "this incomming call is invalid\n")
	session:hangup()
	return
else
	get_dialrules_by_sipnumber(dnis)
end

get_system_files()
if ( system_fail_times_str == nil or system_fail_times_str == "" ) then
	system_fail_times_str = "1"
end
system_fail_times = tonumber(system_fail_times_str)
if ( system_busy_times_str == nil or system_busy_times_str == "" ) then
	system_busy_times_str = "1"
end
system_busy_times = tonumber(system_busy_times_str)

if ( tonumber(SipNumber.voice_type) == 0 ) then
	if ( system_welcome_file == "" ) then
		if ( dbh_cfg:connected() == false ) then
			freeswitch.consoleLog("INFO", "dbh_cfg is not connected.\n")
		end
		session:sleep(100)
	end
	welcome_file = system_welcome_file
else
	get_userdefined_welcome_file(dnis)
	welcome_file = userdefined_welcome_file
end
get_system_timeout_second()


session:answer()

repeat_times = 0


user_entered_digits = ""
start_or_user_enter_time = os.time()
changed_to_phoneno = ""
mute_flag = false

function collect_digits_cb(session, type, data, arg)
	if type == "dtmf" then
		mute_flag = true
		api = freeswitch.API();
		local callstring = "bgapi uuid_audio "..objectuuid.." start write mute -4"
		freeswitch.consoleLog("notice", callstring.."\n");
		api:executeString(callstring);

		start_or_user_enter_time = os.time()
	user_entered_digits = user_entered_digits .. data["digit"]
    freeswitch.consoleLog("INFO", "Key pressed: " .. data["digit"] .. "    " .. user_entered_digits .. "\n")
	digits_len = string.len(user_entered_digits)

	for i,n in ipairs(dial_rules) do
		if ( n.key == user_entered_digits ) then
			changed_to_phoneno = n.value
			iposx, iposy = string.find(changed_to_phoneno, "I")
			if ( iposx ~= nil ) then
				changed_to_phoneno, _ = string.gsub(changed_to_phoneno, "I", user_entered_digits)
			end
			freeswitch.consoleLog("INFO", "found a math for user entered key:" .. user_entered_digits .. ".   key:" .. n.key .. ",  transfer no:" .. n.value .. "\n")
			return "break"
		elseif ( string.len(n.key) == string.len(user_entered_digits) ) then
			changed_key = n.key
			firstx, firsty = string.find(n.key, "X")
			lastx, lasty = string.find(n.key, "X", -1)
			if ( firstx ~= nil and lastx ~= nil ) then
				freeswitch.consoleLog("INFO", string.format("count:%d\n", lastx - firstx + 1))
				changed_key = string.sub(changed_key, 1, firstx - 1)
				freeswitch.consoleLog("INFO", "changed again  changed key:" .. changed_key .. "\n")
				for i=1, lastx - firstx + 1 do
					changed_key = changed_key .. "%d"
				end
				freeswitch.consoleLog("INFO", "after added %d  changed key:" .. changed_key .. "\n")
				mpos, npos = string.find(user_entered_digits, changed_key)
				if ( mpos ~= nil ) then
					matched_len = npos - mpos + 1
					if ( matched_len == string.len(user_entered_digits) ) then
						freeswitch.consoleLog("INFO", string.format("regex match perfect    m:%d.  n:%d\n", mpos, npos))
						changed_to_phoneno = n.value
						iposx, iposy = string.find(changed_to_phoneno, "I")
						if ( iposx ~= nil ) then
							changed_to_phoneno, _ = string.gsub(changed_to_phoneno, "I", user_entered_digits)
						end
						return "break"
					else
						freeswitch.consoleLog("INFO", string.format("regex match result    m:%d.  n:%d\n", mpos, npos))
					end
				end
			end
		end
	end
  end
	return "true"
end

function myHangupHook(s, status, arg)
	record_call(0)
	freeswitch.consoleLog("INFO", "dbdir:" .. dbdir.."\n")
    	freeswitch.consoleLog("NOTICE", "myHangupHook: " .. status .. "\n")
	return "exit"
end
blah="w00t"
session:setHangupHook("myHangupHook", "blah")

while ( true ) do
	if ( not session:ready() ) then
		return
	end
	session:setInputCallback("collect_digits_cb", "")
	times = 0
	changed_to_phoneno = ""
	while ( times < tonumber(SipNumber.play_times) ) do
		user_entered_digits = ""
		start_or_user_enter_time = os.time()

		if ( mute_flag == true ) then
			api = freeswitch.API();
			local callstring = "bgapi uuid_audio "..objectuuid.." stop"
			freeswitch.consoleLog("notice", callstring.."\n")
			api:executeString(callstring)
		end

		session:streamFile(welcome_file)
		if ( mute_flag == false ) then
			start_or_user_enter_time = os.time()
		end
		freeswitch.consoleLog("INFO", "Got " .. user_entered_digits .. "\n")
		while ( ( string.len(user_entered_digits) == 0 and os.difftime(os.time(), start_or_user_enter_time) < tonumber(system_timeout_second) ) or ( string.len(user_entered_digits) > 0 and os.difftime(os.time(), start_or_user_enter_time) < tonumber(system_timeout_second) ) ) do
			if ( string.len(changed_to_phoneno) > 0 ) then
				break
			end
			session:sleep(50)
		end
		if ( string.len(changed_to_phoneno) > 0 ) then
			break
		else
			if ( string.len(SipNumber.default_no) == 0 and string.len(user_entered_digits) > 0 ) then
				changed_to_phoneno = user_entered_digits
				break
			end		
		end
		times = times + 1
	end
	if ( times >= tonumber(SipNumber.play_times) ) then
		if ( mute_flag == true ) then
			api = freeswitch.API();
			local callstring = "bgapi uuid_audio "..objectuuid.." stop"
			freeswitch.consoleLog("notice", callstring.."\n")
			api:executeString(callstring)

			if ( string.len(SipNumber.default_no) > 0 ) then
				changed_to_phoneno = SipNumber.default_no
			else
				changed_to_phoneno = user_entered_digits
			end
		else
			if ( tonumber(SipNumber.timeout_action) == 1 ) then
				record_call(0)
				session:hangup()
				return
			else
				changed_to_phoneno = SipNumber.transfer_no
			end
		end
	end
	session:unsetInputCallback()

	if ( mute_flag == true ) then
		api = freeswitch.API();
		local callstring = "bgapi uuid_audio "..objectuuid.." stop"
		freeswitch.consoleLog("notice", callstring.."\n")
		api:executeString(callstring)
	end

	session:setVariable("call_timeout", string.format("%d", SipNumber.no_answer_timeout))
	dialB = "[origination_caller_id_number=" .. ani .. ",execute_on_answer=lua tsimplify.lua " .. objectuuid .. " " .. transfer_record.start_date_str .. " " .. transfer_record.start_hhmmss_str .. " " .. transfer_record.ani .. " " .. transfer_record.sipip .. " " .. transfer_record.sipno .. "]sofia/external/" .. changed_to_phoneno .. "@" .. sipsvrip .. ":" .. sipsvrport
	freeswitch.consoleLog("INFO","new session:" .. dialB .. "\n")
	
	legB = freeswitch.Session(dialB, session)
	--freeswitch.bridge(session, legB)
	--original version	legB = freeswitch.Session(dialB, session)
	freeswitch.consoleLog("INFO","after new session..\n")

	state=legB:getState()
	freeswitch.consoleLog("INFO","new session state:"..state.."\n")
	obCause=legB:hangupCause()
	freeswitch.consoleLog("INFO","new session state:"..state.."    hangup cause:"..obCause.."\n")

	if ( state == "ERROR" and ( obCause == "USER_BUSY" ) ) then
		repeat_digits = session:playAndGetDigits(1, 1, tonumber(system_busy_times), tonumber(system_timeout_second) * 1000, "", system_busy_file, "", "[*]")
		if ( repeat_digits == "" or repeat_digits ~= "*" ) then
			record_call(0)
			session:hangup()
			return
		end
	elseif ( state == "ERROR" and ( obCause == "NO_ANSWER" or obCause == "UNALLOCATED_NUMBER" or obCause == "DESTINATION_OUT_OF_ORDER" or obCause == "FACILITY_REJECTED" or obCause == "NORMAL_CIRCUIT_CONGESTION" or obCause == "NETWORK_OUT_OF_ORDER" or obCause == "NORMAL_TEMPORARY_FAILURE" or obCause == "SWITCH_CONGESTION" or obCause == "REQUESTED_CHAN_UNAVAIL" or obCause == "BEARERCAPABILITY_NOTAVAIL" or obCause == "FACILITY_NOT_IMPLEMENTED" or obCause == "SERVICE_NOT_IMPLEMENTED" or obCause == "RECOVERY_ON_TIMER_EXPIRE" or obCause == "INVALID_NUMBER_FORMAT" ) ) then
		repeat_digits = session:playAndGetDigits(1, 1, tonumber(system_fail_times), tonumber(system_timeout_second) * 1000, "", system_fail_file, "", "[*]")
		if ( repeat_digits == "" or repeat_digits ~= "*" ) then
			record_call(0)
			session:hangup()
			return
		end
	elseif ( legB:ready() ) then
		originate_end_time = os.time()
		while ( os.difftime(os.time(), originate_end_time) < tonumber(30) ) do
			session:sleep(500)
		end
		return
	end
end		-- end of while
