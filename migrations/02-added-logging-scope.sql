DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'logs' AND column_name = 'scope'
    ) THEN 
        ALTER TABLE public.logs ADD COLUMN scope VARCHAR(255);
        CREATE INDEX IF NOT EXISTS ix_logs_scope ON public.logs (scope);
    END IF;
END$$;

INSERT INTO public.__migrations("number", name, date)
VALUES (102, 'Added log scoping', CURRENT_DATE)
ON CONFLICT ("number") DO NOTHING;
