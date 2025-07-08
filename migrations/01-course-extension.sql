CREATE TABLE IF NOT EXISTS public.course_revision_dictionary_words
(
    id                 UUID DEFAULT gen_random_uuid() NOT NULL,
    course_revision_id INTEGER                        NOT NULL, 
    word               VARCHAR(255)                   NOT NULL,
    explanation        TEXT                           NOT NULL,
    language           VARCHAR(3)                     NOT NULL,
    picture_url        VARCHAR(511),
    CONSTRAINT course_revision_words_pk PRIMARY KEY (id),
    CONSTRAINT course_revision_words_course_revisions_id_fk
        FOREIGN KEY (course_revision_id) REFERENCES public.course_revisions
        ON UPDATE CASCADE ON DELETE CASCADE
);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'course_revisions' AND column_name = 'general_information'
    ) THEN
        ALTER TABLE public.course_revisions
        ADD COLUMN general_information TEXT;
    END IF;
END$$;

INSERT INTO public.__migrations("number", name, date)
VALUES (101, 'Course with general information', CURRENT_DATE)
ON CONFLICT ("number") DO NOTHING;
