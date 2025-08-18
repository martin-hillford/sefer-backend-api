DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'users' AND column_name = 'additional_info'
    ) THEN
        ALTER TABLE public.users ADD COLUMN additional_info jsonb;        
    END IF;
END$$;

-- create a view with mentor performance --
CREATE OR REPLACE VIEW mentor_performance AS
SELECT average_messages.mentor_id, average_message_per_student, review_time_span, days_active, rating.average_rating, rating.rating_count
FROM
(
    SELECT sender_id AS mentor_id, AVG(message_count) AS average_message_per_student
    FROM
        (
            SELECT sender_id, COUNT(id) AS message_count
            FROM chat_messages
            GROUP BY sender_id, channel_id
        ) AS messages
    GROUP BY sender_id
) AS average_messages
    JOIN
(
    SELECT
        AVG(EXTRACT(EPOCH FROM (submission_date - review_date))) AS review_time_span,
        enrollments.mentor_id
    FROM lesson_submissions
             JOIN enrollments ON enrollments.id = lesson_submissions.enrollment_id
    WHERE is_final = TRUE AND review_date IS NOT NULL AND submission_date IS NOT NULL
    GROUP BY enrollments.mentor_id
) AS review_performance
ON review_performance.mentor_id = average_messages.mentor_id
    LEFT JOIN
(
    SELECT COUNT(date) AS days_active, user_id AS mentor_id
    FROM
        (
            SELECT DISTINCT user_id, date FROM
                (
                    SELECT DISTINCT sender_id AS user_id, CAST(sender_date AS DATE) AS date
                    FROM chat_messages
                    WHERE sender_date > (clock_timestamp() - INTERVAL '1 year')
                    GROUP BY sender_id,  CAST(sender_date AS DATE)
                    UNION
                    SELECT DISTINCT user_id, CAST(log_time AS DATE) AS date
                    FROM login_log_entries
                    WHERE log_time > (clock_timestamp() - INTERVAL '1 year')
                    UNION
                    SELECT DISTINCT mentor_id AS user_id, CAST(review_date AS DATE)
                    FROM lesson_submissions
                             JOIN enrollments ON enrollments.id = lesson_submissions.enrollment_id
                    WHERE review_date > (clock_timestamp() - INTERVAL '1 year')
                )
                    AS activities
        ) AS D
    GROUP BY user_id
) AS activity
ON activity.mentor_id = average_messages.mentor_id
    LEFT JOIN
(
    SELECT AVG(CAST(rating as Float)) AS average_rating, COUNT(id) AS rating_count,  mentor_id
    FROM mentor_ratings
    GROUP BY mentor_id
) AS rating
ON rating.mentor_id = average_messages.mentor_id;

INSERT INTO public.__migrations("number", name, date)
VALUES (104, 'Extended Mentor Performance', CURRENT_DATE)
ON CONFLICT ("number") DO NOTHING;