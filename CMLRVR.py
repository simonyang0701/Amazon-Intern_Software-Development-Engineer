#!/usr/bin/python3
# -*- coding: utf-8 -*-

'''
Copyright (c) 2017, Amazon Lab126
All rights reserved.

Author: simon yang
Reference: demi wang


'''

import json
import urllib
import boto3
import os
import datetime
import time
import subprocess
import pymysql
import pandas as pd
import csv
import logging
from logging.handlers import RotatingFileHandler

logFormatter = logging.Formatter('%(asctime)s %(levelname)s %(message)s')
logFile="CMLRVR.log"
logHandler = RotatingFileHandler(logFile, mode='a', maxBytes=1024*1024*7, backupCount=2, encoding=None, delay=0)

logHandler.setFormatter(logFormatter)
logHandler.setLevel(logging.INFO)

logger = logging.getLogger('lens-CMLRVR')
logger.setLevel(logging.INFO)

logger.addHandler(logHandler)
'''
logging.basicConfig(level=logging.INFO,
					format='%(asctime)s %(filename)s[line:%(lineno)d] %(levelname)s %(message)s',
					datefmt='%a, %d %b %Y %H:%M:%S',
					filename='/home/ubuntu/CMLRVR.log',
					filemode='w')
'''

#from airConstant import *
db_endpoint = 'lens-cluster.cluster-c3jg1eun6ejf.us-west-2.rds.amazonaws.com'
db_username = 'lens'
db_password = 'password'
db_name = 'LENS'
db_port = 3306
def is_float(s):
	try:
		logger.info('it is float or not')
		float(s)
		logger.info('is float')
		return True
	except:
		logger.info('is not float')
		return False

def get_size(start_path = '/home/ubuntu/tmp_cmlrvr/'):
	total_size = 0
	for dirpath, dirnames, filenames in os.walk(start_path):
		for f in filenames:
			fp = os.path.join(dirpath, f)
			total_size += os.path.getsize(fp)
	return total_size

def cleanup():
	import shutil
	logger.info("ls /home/ubuntu/tmp_cmlrvr")
	try:
		ret = subprocess.check_output(["ls", "-alR", "/home/ubuntu/tmp_cmlrvr"], stderr=subprocess.STDOUT)
		logger.info(ret)
	except Exception as e:
		logger.info(e)

	logger.info(get_size())

	logger.info("clean up /home/ubuntu/tmp_cmlrvr")
	for root, dirs, files in os.walk('/home/ubuntu/tmp_cmlrvr/'):
		for f in files:
			os.unlink(os.path.join(root,f))
		for d in dirs:
			shutil.rmtree(os.path.join(root,d))

	logger.info("ls /home/ubuntu/tmp_cmlrvr")
	try:
		ret = subprocess.check_output(["ls", "-alR", "/home/ubuntu/tmp_cmlrvr"], stderr=subprocess.STDOUT)
		logger.info(ret)
	except Exception as e:
		logger.info(e)

	logger.info(get_size())

while True:
	with open('/home/ubuntu/run', 'r') as f:
		fileconext = f.read()
		if fileconext.find('0')!=-1:
			logger.info('find 0 in run file')
			break
	try:
		db_conn = pymysql.connect(db_endpoint, user=db_username,passwd=db_password, db=db_name, connect_timeout=10)
	except:
		logger.info("ERROR: Unexpected error: Could not connect to MySql instance.")
		sys.exit()
	#logger.info("INFO: connected to MySql instance.")
	logger.info('Loading function')

	field_db = pd.read_sql_query("SELECT Bucket,S3Key, Secret FROM LENS.S3_INFO",db_conn)
	#logger.info(field_db)
	if field_db.empty:
		logger.info('Loading s3 bucket key secret error')
		continue
	bucket =''
	s3key=''
	secret=''
	for index,row in field_db.iterrows():
		bucket = row[0]
		s3key=row[1]
		secret=row[2]
	s3 = boto3.resource('s3',aws_access_key_id=s3key,aws_secret_access_key=secret, region_name='us-west-2')
	#logger.info('Loading S3 function')
	#query LOG table 
	if db_conn is None:
		logger.info('database is disconnected')
		continue
	
	field_db = pd.read_sql_query("SELECT log,logsize,uploadtime FROM LENS.LOG WHERE processed = '0'",db_conn)
	#logger.info(field_db)
	if field_db.empty:
		logger.info('return : SELECT log,logsize FROM LENS.LOG WHERE processed = 0 is empty')
		continue

	logpath_s3=[]
	logsize_s3=[]
	loguploadtime_s3=[]
	Okfile=1
	ErrMessage=''
	for index,row in field_db.iterrows():
		ErrMessage=''
		Okfile=1
		logger.info(row[0])
		logger.info(row[1])
		if str(row[0]).find("CMLRVR")!=-1 and str(row[0]).endswith(".csv"):
			logpath=row[0]
			filename = os.path.basename(logpath)
			fn = filename.split('_')
			logger.info(filename)
			if (len(fn) <3 ):
				logger.info("wrong file %s" % filename)
				ErrMessage='cmlrvr file name is wrong'
				Okfile=0
			if Okfile==1:
				logsize_s3.append(row[1])
				logpath_s3.append(row[0])
				loguploadtime_s3.append(row[2])
			else:
				logger.info('update LENS.LOG processed=2')
				db_update_cmd ='UPDATE LENS.LOG SET processed=\'2\',errmessage=\''+ErrMessage+'\' where log =\''+logpath+'\''
				logger.info(db_update_cmd)
				with db_conn.cursor() as cur:
					cur.execute(db_update_cmd)
					db_conn.commit()
	if len(logpath_s3)<1:
		continue
	processdata=0
	LogNum= len(logpath_s3)
	ErrorMessage=[''] * LogNum
	product=[''] * LogNum
	dsn=[''] * LogNum
	time_fn=[''] * LogNum
	description=[''] * LogNum
	k=0
	while(k<LogNum):
		try:
			processdata=0
			filename = os.path.basename(logpath_s3[k])
			filename = filename[:-4]
			fn = filename.split('_')
			product[k]=fn[0]
			dsn[k]=fn[1]
			time_fn[k] = fn[2]
			try:
				time1 = time.strptime(time_fn[k],"%Y.%m.%d-%H.%M.%S")
				time1 = time.strftime("%Y.%m.%d-%H.%M.%S",time1)
			except:
				ErrorMessage[k]='except abmormal timestamp:'+time_fn[k]
				logger.info(ErrorMessage[k])
				processdata=2
				continue
			if len(fn)>=4:
				description[k] =fn[3]
			else:
				description[k]=''
			logger.info(product[k])
			logger.info(dsn[k])
			logger.info(time_fn[k])
			logger.info(description[k])
			tmpdir= '/home/ubuntu/tmp_cmlrvr'
			FileName = os.path.join(tmpdir, os.path.basename(logpath_s3[k]))
			logger.info('File name:'+ FileName)
			try:
				s3.meta.client.download_file(bucket, logpath, FileName)
			except Exception as e:
				logger.info(e)
				logger.info('Error getting object {} from bucket {}. Make sure they exist and your bucket is in the same region as this function.'.format(logpath, bucket))
				ErrorMessage[k]+='download file from S3 failed'
				processdata=2
				continue
			tmpdir=os.path.dirname(FileName)
			logger.info(tmpdir)
			if not os.path.exists(FileName):
				logger.info("wrong file %s" % FileName)
				ErrorMessage[k]+= "wrong file %s" % FileName
				processdata=2
				continue
			
			if FileName.endswith('.csv'):
				#get these 6 logpaths from csv for locating testcaseid
				logger.info('tracking')
				with open(FileName,'r') as csvfile:
					reader = csv.DictReader(csvfile)
					Aplocation_lut_cn = [row['APLocation'] for row in reader]
					logging.info(Aplocation_lut_cn)
					Aplocation_lut_cn = list(set(Aplocation_lut_cn))
					logging.info(Aplocation_lut_cn)
				with open(FileName,'r') as csvfile:
					reader = csv.DictReader(csvfile)
					Profile_lut_cn = [row['Profile'] for row in reader]
					logging.info(Profile_lut_cn)
					Profile_lut_cn = list(set(Profile_lut_cn))
					logging.info(Profile_lut_cn)
				with open(FileName,'r') as csvfile:
					reader = csv.DictReader(csvfile)
					TestItem_lut_cn = [row['TestItem'] for row in reader]
					logging.info(TestItem_lut_cn)
					TestItem_lut_cn = list(set(TestItem_lut_cn))
					logging.info(TestItem_lut_cn)
				with open(FileName,'r') as csvfile:
					reader = csv.DictReader(csvfile)
					fw = [row['FW_version'] for row in reader]
					logging.info(fw)
					fw = list(set(fw))
					logging.info(fw[0])
				with open(FileName,'r') as csvfile:
					reader = csv.DictReader(csvfile)
					Band_lut_cn = [row['Band'] for row in reader]
					logger.info(Band_lut_cn)
					Band_lut_cn = list(set(Band_lut_cn))
					logger.info(Band_lut_cn)
				#get testcaseid
				TestCaseID={}
				for aplocation in Aplocation_lut_cn:
					for profile in Profile_lut_cn:
						for testitem in TestItem_lut_cn:
							for band in Band_lut_cn:
								db_fields = "`APLocation`='"+aplocation+"' and Profile='"+profile+"' and TestItem ='"+testitem+"' and Band ='"+band+"'"
								strTestCaseID = pd.read_sql_query("SELECT TestCaseID FROM LENS.CMLRVR_LUT WHERE "+db_fields,db_conn)						
								logpathfordict=aplocation+'_'+profile+'_'+testitem+'_'+band
								logger.info(logpathfordict)
								logger.info(db_fields)
								logger.info(strTestCaseID)
								if strTestCaseID.empty:
									logger.info('strTestCaseID is empty')
									ErrorMessage[k]+='strTestCaseID is empty'
									processdata=2
									continue
								for index,row in strTestCaseID.iterrows():
									logger.info(row[0])
									ChecktestcaseinTCS = pd.read_sql_query('select distinct ExecutionStatus,TestTime,DSN from LENS.'+product[k]+'_TCS where TestCaseID=\''+row[0]+'\'',db_conn)
									if ChecktestcaseinTCS.empty:
										ErrorMessage[k]+='can not find testcase Id in '+product[k]+'_TCS'
										logger.info(ErrorMessage[k])
										continue
									tempdict={logpathfordict:row[0]}
									TestCaseID.update(tempdict)
									logger.info("This is TestCaseID")
									logger.info(TestCaseID)
				if len(TestCaseID)<1:
					ErrorMessage[k]+='can not find testcase Id'
					logger.info('TestCaseID is empty,can not find testcase Id')
					processdata=2
					continue
				#get cn_name and cn_type of LEN.CMLRVR
				cn_name_db=[]
				cn_type_db=[]
				if db_conn is None:
					pass
				else:
					field_db = pd.read_sql_query("SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'CMLRVR'",db_conn)
					logger.info(field_db)
					for index,row in field_db.iterrows():
						logger.info(row[0])
						logger.info(row[1])
						cn_name_db.append(row[0])
						cn_type_db.append(row[1])
					logger.info(cn_name_db)
					logger.info(cn_type_db)
				#get hearder of csv
				cn_name_csv=[]
				cn_data_csv=[]
				i=0
				with open(FileName,'r') as csvfile:
					reader = csv.reader(csvfile)
					for row in reader:
						i+=1
						logger.info(row)
						if i==1:
							cn_name_csv=row
						if i==2:
							cn_data_csv=row
							break
				i=0
				add_column=[]
				while(i<(len(cn_name_csv))):
					logger.info(cn_name_csv[i])
					if cn_data_csv[i].strip()=='0x1008':
						logger.info('data is empty!')
					else:
						if cn_name_csv[i] not in cn_name_db:
							logger.info('add '+cn_name_csv[i]+' into database')
							# add the fileds
							if db_conn is None:
								pass
							else:
								with db_conn.cursor() as cur:
									add_column.append(cn_name_csv[i])
									isfloat = is_float(cn_data_csv[i])
									if isfloat:
										sql = "alter table LENS.CMLRVR add `"+cn_name_csv[i]+"` DOUBLE"
									else:
										sql = "alter table LENS.CMLRVR add `"+cn_name_csv[i]+"` VARCHAR(100)"
									logger.info('SQL:'+sql)
									cur.execute(sql)
								db_conn.commit()
					i+=1
				#check new column is wrote
				cn_name_db=[]
				cn_type_db=[]
				if db_conn is None:
					pass
				else:
					field_db = pd.read_sql_query("SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'CMLRVR'",db_conn)
					logger.info(field_db)
					for index,row in field_db.iterrows():
						logger.info(row[0])
						logger.info(row[1])
						cn_name_db.append(row[0])
						cn_type_db.append(row[1])
					logger.info(cn_name_db)
					logger.info(cn_type_db)
				i=0
				addnewcolumndone=1
				logger.info(add_column)
				while(i<len(add_column)):
					if add_column[i] not in cn_name_db:
						logger.info('new column:'+add_column[i]+'  has bot been added')
						ErrorMessage[k]+='new column:'+add_column[i]+'  has bot been added'
						addnewcolumndone=0
					i+=1
				if addnewcolumndone==0:
					logger.info('processdata=2')
					processdata=2
					continue
				#insert data to db
				logger.info('start insert data to db')
				totalrow=0
				with open(FileName,'r') as csvfile:
					rows =csvfile.readlines()
					totalrow = len(rows)
					logger.info('totalrow')
					logger.info(totalrow)
					i=0
					for item in row:
						if 'CML' in item:
							i=i+1
					logger.info('i = ')
					logger.info(i)
				with open(FileName,'r') as csvfile:
					reader = csv.DictReader(csvfile)
					executedrow=0
					logger.info('track')
					for row in reader:				
						db_tpye=''
						db_fields=''
						db_value=''
						logger.info(row)
						#get testcaseid
						logpath_dict= row['APLocation']+'_'+row['Profile']+'_'+row['TestItem']+'_'+row['Band']
						logger.info('logpath_dict:'+logpath_dict)
						try:
							testcaset_id = TestCaseID[logpath_dict]
						except:
							ErrorMessage[k]+='can not find testcase Id in TestCaseID dict'
							logger.info(ErrorMessage[k])
							continue
						logger.info('testcaset_id: '+testcaset_id)
						i=1
						logger.info('len(cn_name_db):'+str(len(cn_name_db)))
						while(i<len(cn_name_db)):
							logger.info(i)
							logger.info('cn_name_db[i]:'+cn_name_db[i])
							db_fields+='`'+cn_name_db[i]+'`,'
							logger.info(db_fields)
							if cn_name_db[i] in cn_name_csv:
								db_value+='\''+row[cn_name_db[i]]+'\','
							elif cn_name_db[i]=='TestCaseID':
								db_value+='\''+testcaset_id+'\','
							elif cn_name_db[i]=='ProjectName':
								db_value+='\''+product[k]+'\','	
							elif cn_name_db[i]=='DSN':
								db_value+='\''+dsn[k]+'\','
							elif cn_name_db[i]=='TestTime':
								db_value+='\''+time_fn[k]+'\','
							elif cn_name_db[i]=='s3log_path':
								db_value+='\''+logpath+'\','
							elif cn_name_db[i]=='UploadTime':
								db_value+='\''+str(loguploadtime_s3[k])+'\','	
							else:
								db_value+='\' \','						
							logger.info(db_value)
							i+=1
						db_fields=db_fields[:-1]
						logger.info('db_fields:'+db_fields)
						db_value=db_value[:-1]
						logger.info('db_value:'+db_value)
						db_cmd = 'insert into LENS.CMLRVR ('+db_fields+')'+' values('+db_value+')'
						logger.info('db_cmd:'+db_cmd)
						if db_conn is None:
							pass
						else:
							with db_conn.cursor() as cur:
								cur.execute(db_cmd)
								db_conn.commit()
								executedrow+=1
								logger.info(executedrow)
								logger.info(totalrow)
					if executedrow==totalrow-16:
						processdata=1
						
				if processdata==1:
					for keys,value in TestCaseID.items():
						Dorecords=False
						logger.info(value)
						executestatus_testcaseid_time = pd.read_sql_query('select distinct ExecutionStatus,TestTime,DSN,FW from LENS.'+product[k]+'_TCS where TestCaseID=\''+value+'\'',db_conn)
						if executestatus_testcaseid_time.empty:
							Dorecords=False
							continue
						executestatus_testcaseid_time.column=['ExecutionStatus','TestTime','DSN','FW']
						logger.info(executestatus_testcaseid_time)
						ExecutionStatus_tcs=[]
						TimeInfo_tcs=[]
						DSN_tcs=[]
						FW_tcs=[]
						for index,row in executestatus_testcaseid_time.iterrows():
							ExecutionStatus_tcs.append(row[0])
							TimeInfo_tcs.append(row[1])
							DSN_tcs.append(row[2])
							FW_tcs.append(row[3])
							Dorecords= True
						i=0
						time_temp = time_fn[k]
						while(i<len(ExecutionStatus_tcs)):
							logger.info('str(TimeInfo_tcs[i])'+str(TimeInfo_tcs[i]))
							logger.info('str(DSN_tcs[i])'+str(DSN_tcs[i]))
							logger.info('str(FW_tcs[i])'+str(FW_tcs[i]))
							logger.info('time_fn'+time_fn[k])
							logger.info('dsn'+dsn[k])
							logger.info('fw[0]'+str(fw[0]))
							array = time_temp.split('-')
							time1=array[0].replace('.','-')
							time2=array[1].replace('.',':')
							time3=time1+' '+time2
							logger.info('time3:'+time3)
							if str(TimeInfo_tcs[i]) == time3 and str(DSN_tcs[i])==dsn[k] and str(FW_tcs[i])==str(fw[0]):
								logger.info('duplicate records!! will not record to TCS again')
								Dorecords = False
								break
							i+=1
						if Dorecords==True and len(ExecutionStatus_tcs)>=1:
							if str(ExecutionStatus_tcs[0])=='Done':
								logger.info('this testcaseid have done before! copy exist row to new row and update it!')
								cn_name_tcsdb=[]
								if db_conn is None:
									pass
								else:
									field_db = pd.read_sql_query('SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = \''+product[k]+'_TCS\'',db_conn)
									logger.info(field_db)
									for index,row in field_db.iterrows():
										logger.info(row[0])
										cn_name_tcsdb.append(row[0])
									logger.info(cn_name_tcsdb)
								x=0
								oldrow=''
								newrow=''
								while(x<len(cn_name_tcsdb)):
									oldrow += cn_name_tcsdb[x]+','
									if cn_name_tcsdb[x]=='TestTime':
										newrow+='\''+time_fn[k]+'\','
									elif cn_name_tcsdb[x]=='UploadTime':
										newrow+='\''+str(loguploadtime_s3[k])+'\','
									elif cn_name_tcsdb[x]=='DSN':
										newrow+='\''+dsn[k]+'\','
									elif cn_name_tcsdb[x]=='FW':
										newrow+='\''+fw[0]+'\','
									elif cn_name_tcsdb[x]=='ExecutionStatus':
										newrow+='\'Done\','
									else:
										newrow+=cn_name_tcsdb[x]+','
									x+=1
								newrow =newrow.rstrip(',')
								oldrow = oldrow.rstrip(',')
								db_update_cmd ='insert into LENS.'+product[k]+'_TCS('+oldrow+') select '+newrow+' from LENS.'+product[k]+'_TCS where TestCaseID=\''+value+'\' and TestTime=\''+str(TimeInfo_tcs[0])+'\''
								logger.info(db_update_cmd)
								if db_conn is None:
									pass
								else:
									with db_conn.cursor() as cur:
										cur.execute(db_update_cmd)
										db_conn.commit()
							else:
								logger.info('this testcaseid have not done before!')
								db_update_cmd ='UPDATE LENS.'+product[k]+'_TCS SET ExecutionStatus=\'Done\', TestTime=\''+time_fn[k]+'\',UploadTime=\''+str(loguploadtime_s3[k])+'\',DSN=\''+dsn[k]+'\', FW=\''+fw[0]+'\' where TestCaseID =\''+value+'\''
								logger.info(db_update_cmd)
								if db_conn is None:
									pass
								else:
									with db_conn.cursor() as cur:
										cur.execute(db_update_cmd)
										db_conn.commit()
		finally:
			if findcsv ==0:
				processdata=2
				ErrorMessage[k]+="this packet does not include csv file"
			if processdata==1:
				logger.info('update LENS.LOG processed=1')
				db_update_cmd ='UPDATE LENS.LOG SET processed=\'1\' where log =\''+logpath_s3[k]+'\''
				logger.info(db_update_cmd)
				if db_conn is None:
					pass
				else:
					with db_conn.cursor() as cur:
						cur.execute(db_update_cmd)
						db_conn.commit()
			if processdata==2:
				logger.info('update LENS.LOG processed=2')
				db_update_cmd ='UPDATE LENS.LOG SET processed=\'2\',errmessage=\''+ErrorMessage[k]+'\' where log =\''+logpath_s3[k]+'\''
				logger.info(db_update_cmd)
				with db_conn.cursor() as cur:
					cur.execute(db_update_cmd)
					db_conn.commit()
		
			#clean up /home/ubuntu/tmp_cmlrvr
			cleanup()
			dirs =os.listdir(tmpdir)
			for file in dirs:
				logger.info(file)
				fullname = os.path.join(tmpdir,file)
				if os.path.isfile(fullname):
					logger.info(fullname+ 'it is a file')
				elif os.path.isdir(fullname):
					logger.info(fullname+'it is a dir')
				else:
					logger.info('it is a special file ')
		k+=1
	db_conn.close()
	



