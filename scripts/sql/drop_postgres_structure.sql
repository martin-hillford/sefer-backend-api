DO
$$
    DECLARE r RECORD;
    BEGIN
        -- Loop through all tables in the current schema
        FOR r IN
            SELECT tablename
            FROM pg_tables
            WHERE schemaname = 'public' OR schemaname = 'bibles'
            LOOP
                -- Dynamically execute DROP TABLE for each table
                EXECUTE 'DROP TABLE IF EXISTS ' || quote_ident(r.tablename) || ' CASCADE';
            END LOOP;
    END
$$;
