DO
$$
    DECLARE stmt TEXT;
    BEGIN
        SELECT INTO stmt
            'TRUNCATE TABLE ' || string_agg(format('%I.%I', schemaname, tablename), ', ') || ' CASCADE'
        FROM pg_tables
        WHERE schemaname = current_schema();
        EXECUTE stmt;
    END
$$;