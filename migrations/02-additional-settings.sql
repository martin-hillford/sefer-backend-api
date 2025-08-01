DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'settings' AND column_name = 'strict_gender_assignment'
    ) THEN
        ALTER TABLE public.settings ADD COLUMN strict_gender_assignment BOOLEAN NOT NULL DEFAULT FALSE;
    END IF;
END$$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'settings' AND column_name = 'assign_personal_mentor_on_registration'
    ) THEN
        ALTER TABLE public.settings ADD COLUMN assign_personal_mentor_on_registration BOOLEAN NOT NULL DEFAULT FALSE;
    END IF;
END$$;

INSERT INTO public.__migrations("number", name, date)
VALUES (102, 'Personal Mentor Assignment on Registration', CURRENT_DATE)
ON CONFLICT ("number") DO NOTHING;
