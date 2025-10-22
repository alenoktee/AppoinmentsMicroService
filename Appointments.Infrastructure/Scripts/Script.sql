-- DROP SCHEMA public;

CREATE SCHEMA public AUTHORIZATION pg_database_owner;

-- DROP TABLE public."Appointments";

CREATE TABLE public."Appointments" (
	"Id" uuid NOT NULL,
	"PatientId" uuid NOT NULL,
	"DoctorId" uuid NOT NULL,
	"ServiceId" uuid NOT NULL,
	"Date" date NOT NULL,
	"Time" time NOT NULL,
	"Status" int2 NOT NULL,
	"ServiceName" text NOT NULL,
	"DoctorFirstName" text NOT NULL,
	"DoctorLastName" text NOT NULL,
	"DoctorMiddleName" text NULL,
	"PatientFirstName" text NOT NULL,
	"PatientLastName" text NOT NULL,
	"PatientMiddleName" text NULL,
	"OfficeId" uuid NULL,
	CONSTRAINT "Appointments_pkey" PRIMARY KEY ("Id")
);



-- DROP TABLE public."Results";

CREATE TABLE public."Results" (
	"Id" uuid NOT NULL,
	"AppointmentId" uuid NOT NULL,
	"Complaints" text NOT NULL,
	"Conclusion" text NOT NULL,
	"Recommendations" text NOT NULL,
	CONSTRAINT "Results_pkey" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_Results_Appointments_AppointmentId" FOREIGN KEY ("AppointmentId") REFERENCES public."Appointments"("Id") ON DELETE CASCADE
);



-- DROP FUNCTION public.create_appointment(uuid, uuid, uuid, uuid, uuid, date, time, int4, text, text, text, text, text, text, text);

CREATE OR REPLACE FUNCTION public.create_appointment(id uuid, patient_id uuid, doctor_id uuid, service_id uuid, office_id uuid, date date, "time" time without time zone, status integer, service_name text, doctor_first_name text, doctor_last_name text, doctor_middle_name text, patient_first_name text, patient_last_name text, patient_middle_name text)
 RETURNS uuid
 LANGUAGE plpgsql
AS $function$
BEGIN
    INSERT INTO "Appointments" ("Id", "PatientId", "DoctorId", "ServiceId", "OfficeId", "Date", "Time", "Status", "ServiceName", "DoctorFirstName", "DoctorLastName", "DoctorMiddleName", "PatientFirstName", "PatientLastName", "PatientMiddleName")
    VALUES (id, patient_id, doctor_id, service_id, office_id, "date", "time", status, service_name, doctor_first_name, doctor_last_name, doctor_middle_name, patient_first_name, patient_last_name, patient_middle_name);
    RETURN id;
END;
$function$
;

-- DROP FUNCTION public.create_appointment(uuid, uuid, uuid, uuid, uuid, date, time, text, text, text, text, text, text, text);

CREATE OR REPLACE FUNCTION public.create_appointment(id uuid, patient_id uuid, doctor_id uuid, service_id uuid, office_id uuid, date date, "time" time without time zone, service_name text, doctor_first_name text, doctor_last_name text, doctor_middle_name text, patient_first_name text, patient_last_name text, patient_middle_name text)
 RETURNS uuid
 LANGUAGE plpgsql
AS $function$
BEGIN
    INSERT INTO "Appointments" ("Id", "PatientId", "DoctorId", "ServiceId", "OfficeId", "Date", "Time", "ServiceName", "DoctorFirstName", "DoctorLastName", "DoctorMiddleName", "PatientFirstName", "PatientLastName", "PatientMiddleName")
    VALUES (id, patient_id, doctor_id, service_id, office_id, "date", "time", service_name, doctor_first_name, doctor_last_name, doctor_middle_name, patient_first_name, patient_last_name, patient_middle_name);
    RETURN id;
END;
$function$
;

-- DROP FUNCTION public.create_result(uuid, uuid, text, text, text);

CREATE OR REPLACE FUNCTION public.create_result(p_id uuid, p_appointment_id uuid, p_complaints text, p_conclusion text, p_recommendations text)
 RETURNS SETOF "Results"
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    INSERT INTO "Results" ("Id", "AppointmentId", "Complaints", "Conclusion", "Recommendations")
    VALUES (p_id, p_appointment_id, p_complaints, p_conclusion, p_recommendations)
    RETURNING *;
END;
$function$
;

-- DROP FUNCTION public.delete_appointment(uuid);

CREATE OR REPLACE FUNCTION public.delete_appointment(appointment_id uuid)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
BEGIN
    DELETE FROM "Appointments" WHERE "Id" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.get_appointment_for_doctor(uuid);

CREATE OR REPLACE FUNCTION public.get_appointment_for_doctor(appointment_id uuid)
 RETURNS TABLE("Id" uuid, "Date" date, "Time" time without time zone, "PatientFirstName" text, "PatientLastName" text, "ServiceName" text, "Status" smallint)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY SELECT a."Id", a."Date", a."Time", a."PatientFirstName", a."PatientLastName", a."ServiceName", a."Status" FROM "Appointments" a WHERE a."Id" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.get_appointment_for_patient(uuid);

CREATE OR REPLACE FUNCTION public.get_appointment_for_patient(appointment_id uuid)
 RETURNS TABLE("Id" uuid, "Date" date, "Time" time without time zone, "DoctorFirstName" text, "DoctorLastName" text, "ServiceName" text, "Status" smallint)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY SELECT a."Id", a."Date", a."Time", a."DoctorFirstName", a."DoctorLastName", a."ServiceName", a."Status" FROM "Appointments" a WHERE a."Id" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.get_appointment_for_receptionist(uuid);

CREATE OR REPLACE FUNCTION public.get_appointment_for_receptionist(appointment_id uuid)
 RETURNS TABLE("Id" uuid, "Date" date, "Time" time without time zone, "PatientFirstName" text, "PatientLastName" text, "PatientMiddleName" text, "DoctorFirstName" text, "DoctorLastName" text, "DoctorMiddleName" text, "ServiceName" text, "PatientPhoneNumber" text, "Status" smallint, "OfficeId" uuid)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY SELECT a."Id", a."Date", a."Time", a."PatientFirstName", a."PatientLastName", a."PatientMiddleName", a."DoctorFirstName", a."DoctorLastName", a."DoctorMiddleName", a."ServiceName", '123-456-7890', a."Status", a."OfficeId" FROM "Appointments" a WHERE a."Id" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.get_appointments_for_date(date);

CREATE OR REPLACE FUNCTION public.get_appointments_for_date(filter_date date)
 RETURNS SETOF "Appointments"
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT * FROM public."Appointments"
    WHERE "Date" = filter_date AND "Status" != 3;
END;
$function$
;

-- DROP FUNCTION public.get_appointments_for_doctor_paginated(uuid, int4, int4, date);

CREATE OR REPLACE FUNCTION public.get_appointments_for_doctor_paginated(doctor_id uuid, page_size integer, page_number integer, filter_date date)
 RETURNS TABLE("Id" uuid, "Date" date, "Time" time without time zone, "PatientFirstName" text, "PatientLastName" text, "ServiceName" text, "Status" smallint)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY SELECT a."Id", a."Date", a."Time", a."PatientFirstName", a."PatientLastName", a."ServiceName", a."Status" FROM "Appointments" a WHERE a."DoctorId" = doctor_id AND a."Date" = filter_date ORDER BY a."Time" ASC LIMIT page_size OFFSET (page_number - 1) * page_size;
END;
$function$
;

-- DROP FUNCTION public.get_appointments_for_patient_paginated(uuid, int4, int4);

CREATE OR REPLACE FUNCTION public.get_appointments_for_patient_paginated(patient_id uuid, page_size integer, page_number integer)
 RETURNS TABLE("Id" uuid, "Date" date, "Time" time without time zone, "DoctorFirstName" text, "DoctorLastName" text, "ServiceName" text, "Status" smallint)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY SELECT a."Id", a."Date", a."Time", a."DoctorFirstName", a."DoctorLastName", a."ServiceName", a."Status" FROM "Appointments" a WHERE a."PatientId" = patient_id ORDER BY a."Date" DESC, a."Time" ASC LIMIT page_size OFFSET (page_number - 1) * page_size;
END;
$function$
;

-- DROP FUNCTION public.get_appointments_for_receptionist_paginated(int4, int4, date, text, text, int2, uuid);

CREATE OR REPLACE FUNCTION public.get_appointments_for_receptionist_paginated(page_size integer, page_number integer, filter_date date DEFAULT NULL::date, doctor_full_name text DEFAULT NULL::text, service_name text DEFAULT NULL::text, filter_status smallint DEFAULT NULL::smallint, office_id uuid DEFAULT NULL::uuid)
 RETURNS TABLE("Id" uuid, "Date" date, "Time" time without time zone, "PatientFirstName" text, "PatientLastName" text, "PatientMiddleName" text, "DoctorFirstName" text, "DoctorLastName" text, "DoctorMiddleName" text, "ServiceName" text, "PatientPhoneNumber" text, "Status" smallint, "OfficeId" uuid)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY SELECT a."Id", a."Date", a."Time", a."PatientFirstName", a."PatientLastName", a."PatientMiddleName", a."DoctorFirstName", a."DoctorLastName", a."DoctorMiddleName", a."ServiceName", '123-456-7890', a."Status", a."OfficeId" FROM "Appointments" a
    WHERE (filter_date IS NULL OR a."Date" = filter_date) AND (doctor_full_name IS NULL OR (a."DoctorLastName" || ' ' || a."DoctorFirstName" || ' ' || a."DoctorMiddleName") ILIKE '%' || doctor_full_name || '%') AND (service_name IS NULL OR a."ServiceName" ILIKE '%' || service_name || '%') AND (filter_status IS NULL OR a."Status" = filter_status) AND (office_id IS NULL OR a."OfficeId" = office_id)
    ORDER BY a."Time" ASC, a."DoctorLastName" ASC, a."DoctorFirstName" ASC, a."ServiceName" ASC LIMIT page_size OFFSET (page_number - 1) * page_size;
END;
$function$
;

-- DROP FUNCTION public.get_occupied_time_slots(uuid, date);

CREATE OR REPLACE FUNCTION public.get_occupied_time_slots(doctor_id uuid, filter_date date)
 RETURNS TABLE("StartTime" time without time zone, "EndTime" time without time zone)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT
        a."Time" AS "StartTime",
        (a."Time" +
            CASE
                WHEN a."ServiceName" ILIKE '%диагностика%' THEN interval '30 minutes'
                WHEN a."ServiceName" ILIKE '%консультация%' THEN interval '20 minutes'
                ELSE interval '10 minutes'
            END
        )::time AS "EndTime"
    FROM
        "Appointments" a
    WHERE
        a."DoctorId" = doctor_id AND a."Date" = filter_date;
END;
$function$
;

-- DROP FUNCTION public.get_result_by_id(uuid);

CREATE OR REPLACE FUNCTION public.get_result_by_id(result_id uuid)
 RETURNS SETOF "Results"
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT * FROM "Results" WHERE "Id" = result_id;
END;
$function$
;

-- DROP FUNCTION public.get_results_by_appointment_id(uuid);

CREATE OR REPLACE FUNCTION public.get_results_by_appointment_id(appointment_id uuid)
 RETURNS SETOF "Results"
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT * FROM "Results" WHERE "AppointmentId" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.reschedule_appointment(uuid, date, time);

CREATE OR REPLACE FUNCTION public.reschedule_appointment(appointment_id uuid, new_date date, new_time time without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    current_status smallint;
BEGIN
    SELECT "Status" INTO current_status FROM "Appointments" WHERE "Id" = appointment_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Appointment with ID % not found', appointment_id USING ERRCODE = 'P0002';
    END IF;
    
    IF current_status != 0 THEN
        RAISE EXCEPTION 'Cannot reschedule an appointment that is not in a scheduled state.' USING ERRCODE = 'P0001';
    END IF;

    UPDATE "Appointments"
    SET
        "Date" = new_date,
        "Time" = new_time
    WHERE "Id" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.update_appointment_status(uuid, int2);

CREATE OR REPLACE FUNCTION public.update_appointment_status(appointment_id uuid, new_status smallint)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
BEGIN
    UPDATE "Appointments" SET "Status" = new_status WHERE "Id" = appointment_id;
END;
$function$
;

-- DROP FUNCTION public.update_result(uuid, text, text, text);

CREATE OR REPLACE FUNCTION public.update_result(p_id uuid, p_complaints text, p_conclusion text, p_recommendations text)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
BEGIN
    UPDATE "Results"
    SET
        "Complaints" = p_complaints,
        "Conclusion" = p_conclusion,
        "Recommendations" = p_recommendations
    WHERE "Id" = p_id;
END;
$function$
;

-- DROP FUNCTION public.update_service_name_in_appointments(uuid, text);

CREATE OR REPLACE FUNCTION public.update_service_name_in_appointments(p_service_id uuid, p_new_name text)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
BEGIN
    UPDATE "Appointments"
    SET "ServiceName" = p_new_name
    WHERE "ServiceId" = p_service_id;
END;
$function$
;
