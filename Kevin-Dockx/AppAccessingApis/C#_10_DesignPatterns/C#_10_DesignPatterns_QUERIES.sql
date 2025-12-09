-- USE TEMP_FCHHOH_IOT_DEV --  ON XBISQL842 -- SELECT TOP 50 * FROM dbo.ReleaseChangescriptLog ORDER BY 3 DESC
-- USE TEMP_FCHHOH_IOT_QA_A1 --  ON XBISQL842 -- SELECT TOP 50 * FROM dbo.ReleaseChangescriptLog ORDER BY 3 DESC
-- USE TEMP_FCHHOH_EH_DEV

-- SELECT TOP 55 * FROM dbo.VENDOR_TRANSACTIONS
-- WHERE vt_Epiid = 38395 AND vt_vteid = 11
-- ORDER BY 1 DESC

-- VENDOR_TRANSACTIONS WITH RELATED
SELECT TOP 55 vt.*, vtp.vtp_description, vit.vit_id, vit.vit_vid, vit.vit_active, SUBSTRING(vt_SessionID, 14, 10) AS SessionID, vit.vit_description, vit.vit_VendorTransactionType -- vit.* -- 
FROM dbo.VENDOR_TRANSACTIONS vt
JOIN VENDOR_TRANSACTION_PROCESSED vtp ON vtp.vtp_id = vt.vt_vtpid
JOIN VENDOR_INTERFACE_TRANSACTIONS vit ON vit.vit_id = vt.vt_vitid
-- WHERE vt.vt_vitid = 138
WHERE vt.vt_vid = 318
-- WHERE vt_Epiid = 38395
-- WHERE vt.vt_EntityId >= 728552
ORDER BY 1 DESC

SELECT TOP 55 li.li_id, li.li_rtid, li.li_iid, li.li_void, li.li_lastupdatedby, li.li_rate, li.li_litid, li.li_transferredfrom_liid, li.li_deleted, li.li_void, li.li_rebill, li.li_includeonclaim, li_covered, li_deleted, li_lastupdate, li.li_insertdate
FROM Billing.LINE_ITEMS li
ORDER BY 1 DESC

RETURN

DECLARE @cevidTop INT = 728822 -- 728426 -- 728424

SELECT cev.CEV_ID, li.li_id, li.li_iid, li.li_void, li.li_lastupdatedby, li.li_rate, li.li_litid, li.li_deleted, li.li_void, li.li_rebill, li.li_includeonclaim, li_covered, li_deleted, li_lastupdate, li.li_insertdate
FROM Billing.LINE_ITEMS li
INNER JOIN 
(
    SELECT rt.rt_id,
    CASE WHEN rt.rt_scheduleid IS NULL THEN (SELECT TOP (1) rt_scheduleid FROM Accounting.REVENUE_TRANSACTIONS WHERE rt_id = rt.rt_from_rtid ORDER BY rt.rt_id) ELSE rt.rt_scheduleid END AS rt_scheduleid
    FROM Accounting.REVENUE_TRANSACTIONS rt
) AccRevTrans ON AccRevTrans.rt_id = li.li_rtid
INNER JOIN dbo.SCHED s ON s.ScheduleID = AccRevTrans.rt_scheduleid
INNER JOIN dbo.CLIENT_SCHED_VISITS csv ON csv.csv_synchid = s.ScheduleID
INNER JOIN dbo.CLIENT_EPISODE_VISITS cev ON cev.CEV_CSVID = csv.csv_id
WHERE cev.CEV_ID = @cevidTop
ORDER BY li.li_insertdate DESC

EXEC usp_Interop_GetBillingLineItemRateByCevId @cevid=@cevidTop
-- EXEC usp_Interop_GetBillingLineItemRateByCevId_TABLEVAR @cevid=@cevidTop -- 
-- EXEC usp_Interop_GetBillingLineItemRateByCevId @cevid = @cevidTop -- 

RETURN

-- Test Data - ANDEZZZRSON, MARGOT (ps_id=28049 (OPTIONS - CLEVELAND), epi_id=-- ep_id=38395) ALT: ANDEZZZRSON, ROSALIE
-- 1. Create Visit, then capture CSVID to DECLARE statment:
SELECT TOP 55 csv_id, csv_paid, csv_insertdate, csv_scheddate, csv_begtime, csv_endtime, * FROM dbo.CLIENT_SCHED_VISITS
ORDER BY 1 DESC

-- 2. Check Payor Setting(s)
SELECT * FROM dbo.PAYOR_SOURCE_FIELDS

SELECT * FROM dbo.PAYOR_SOURCE_FIELD_SETTINGS
WHERE psfs_psid = 28049

EXEC usp_IntSys_GetPayorSourceSettingIsOn 28049, 7

-- 3. Verify Visit, then run the proc to Queue into VENDOR_TRANSACTIONS
-- EXEC usp_IntSys_GetVisitTransactionsFromAuditSched

-- EXEC dbo.usp_ProcessVerifiedVisits
-- 4. Capture CSVID and use to Determine CEVID
DECLARE @csvid INT = 1221203 -- 1220729

SELECT csv.csv_id, csv.csv_scheddate, csv.csv_epiid, csv.csv_agid, csv.csv_epiid, csv.csv_status, csv.csv_visitnumber, csv.csv_synchid, csv.csv_insertdate, 
cev.*
FROM dbo.CLIENT_SCHED_VISITS csv
LEFT JOIN dbo.CLIENT_EPISODE_VISITS cev ON cev.CEV_CSVID = csv.csv_id
WHERE csv.csv_id = @csvid

-- VENDOR_TRANSACTIONS
SELECT TOP 555 * FROM dbo.VENDOR_TRANSACTIONS 
WHERE vt_Epiid = 38395
ORDER BY 1 DESC

-- 5. Create  or Edit Charges & Check if BRS picked up
DECLARE @cevid INT = 728532 -- 728426 -- 728424

SELECT cev.CEV_ID, li.li_id, li.li_iid, li.li_void, li.li_lastupdatedby, li.li_rate, li.li_litid, li.li_deleted, li.li_void, li.li_rebill, li.li_includeonclaim, li_covered, li_deleted, li_lastupdate, li.li_insertdate																					  
FROM Billing.LINE_ITEMS li
INNER JOIN Accounting.REVENUE_TRANSACTIONS rt ON li.li_rtid = rt.rt_id
INNER JOIN dbo.SCHED s ON s.ScheduleID = rt.rt_scheduleid
INNER JOIN dbo.CLIENT_SCHED_VISITS csv ON csv.csv_synchid = s.ScheduleID
INNER JOIN dbo.CLIENT_EPISODE_VISITS cev ON cev.CEV_CSVID = csv.csv_id
WHERE cev.CEV_ID = @cevid
ORDER BY li.li_insertdate DESC


-- (PROCs USED BY BRS)
EXEC usp_Interop_GetBillingLineItemRateByCevId @cevid
EXEC usp_Interop_GetBillingLineItemCevId 1574945
EXEC usp_Interop_GetBillingLineItemRateByCevId 728532
EXEC usp_Interop_GetPayorSourceBySchedID 1138876

EXEC usp_Interop_GetPayorSourceByInvoiceID @invoiceId=133417

-- Look at Submission Status
SELECT TOP 555 * FROM dbo.VENDOR_TRANSACTIONS 
WHERE vt_Epiid = 38395
ORDER BY 1 DESC

-- END Create & Bill a Visit